// import PocketBase from "../vendor/pocketbase/pocketbase.es.mjs";
import PocketBase from "../../vendor/pocketbase/pocketbase.es.mjs";

let pb_url = window.AppConfig?.pocketbaseUrl;

console.log("[editable-store] pb_url:", pb_url);
if (pb_url === null || pb_url === undefined) {
    throw new Error("Pocketbase remote url is not set! (window.AppConfig.pocketbaseUrl)");
}

const pb = new PocketBase(pb_url);
// Prevent auto-cancel of in-flight requests when a new one starts (useful for seed/save)
pb.autoCancellation(false);

export default function createEditableStore({
                                                collection,
                                                field = "markdown",
                                                renderer = (x) => x
                                            }) {
    // IMPORTANT: Do NOT put the `pb` instance on the returned object.
    // Alpine.store() deeply proxies everything. PocketBase has internal
    // circular references / large graphs → Maximum call stack size exceeded.
    // All methods close over the module-level `pb` instead.

    return {
        collection,
        field,

        items: {},
        modalOpen: false,
        editingKey: "",
        draft: "",
        editingType: "content",   // "content" | "images" | "videos"
        sourceStore: null,        // will hold the real store that owns the data

        get isAuthenticated() {
            return pb.authStore.isValid;
        },

        get currentUser() {
            return pb.authStore.record;
        },

        get isAdmin() {
            return this.isAuthenticated && this.currentUser?.role === "admin";
        },

        async init() {
            if (this._loaded && !force) return;          // ← Prevent duplicate loads.
            this._loaded = true;
            try {
                const records = await pb.collection(collection).getFullList();

                // Clear first to preserve reactivity if re-init after login
                Object.keys(this.items).forEach((k) => delete this.items[k]);

                for (const r of records) {
                    this.items[r.key] = r[field];
                }
                console.log(
                    `%c[${collection}] Loaded ${Object.keys(this.items).length} items`,
                    "color:#22c55e"
                );
            } catch (err) {
                console.warn(
                    `[${collection}] init failed (using HTML fallback until seeded/saved):`,
                    err?.message || err
                );
            }
        },

        /**
         * Returns the value if the key exists in the store, otherwise null.
         * null = "not managed by CMS yet" → leave the original element content alone.
         */
        get(key) {
            return key in this.items ? this.items[key] : null;
        },

        /**
         * Seed a missing key (admin only).
         * Checks PB first; creates the record if missing (same spirit as legacy).
         */
        async seed(key, value) {
            if (!key || value == null || value === "") return;
            if (key in this.items) return;

            try {
                const existing = await pb
                    .collection(collection)
                    .getFirstListItem(`key="${key}"`)
                    .catch(() => null);

                if (existing) {
                    let prev = existing[field];
                    console.log(`prev: ${prev}`, prev)
                    if (prev === null ||
                        prev === undefined
                    )
                        return;

                    this.items[key] = existing[field];
                    console.log(`[${collection}] Seeded from existing DB record: ${key}`);
                    return;
                }

                const created = await pb.collection(collection).create({
                    key,
                    [field]: typeof value === "string" ? value.trim() : value,
                });

                this.items[key] = created[field];
                console.log(`[${collection}] Seeded NEW key into PB: ${key}`);
            } catch (err) {
                console.error(`[${collection}] Failed to seed key "${key}"`, err);
                // Fallback so admin can still edit
                this.items[key] = value;
            }
        },

        async save() {
            if (!this.isAdmin || !this.editingKey) return;

            const target = this.sourceStore || this;
            const key = this.editingKey;
            const value = typeof this.draft === "string" ? this.draft.trim() : this.draft;

            try {
                // re-use the existing save logic but against the target store
                const existing = await pb
                    .collection(target.collection)
                    .getFirstListItem(`key="${key}"`)
                    .catch(() => null);

                console.log(`updating ${target.name} - at field ${target.field}`)

                if (existing) {
                    await pb.collection(target.collection).update(existing.id, {
                        [target.field]: value,
                    });
                } else {
                    await pb.collection(target.collection).create({
                        key,
                        [target.field]: value,
                    });
                }

                target.items[key] = value;          // update the real store
                this.modalOpen = false;
                console.log(`[${target.collection}] Saved: ${key}`);
                console.log("with value :>>", value);

            } catch (err) {
                console.error(`Save failed for "${key}"`, err);
                alert("Save failed");
            }
        },

        startEditing({key, type = "content", sourceStore = null}) {
            if (!this.isAdmin) {
                alert("Admin access required");
                return;
            }
            this.editingKey = key;
            this.editingType = type;
            this.sourceStore = sourceStore || this;          // who actually owns the data
            this.draft = (sourceStore || this).get(key) ?? "";
            this.modalOpen = true;
        },

        /**
         * Upload a file directly to PocketBase (images / videos).
         * Uses the `file` field, then writes the resulting public URL into `draft`
         * (and into the `url` field) so the preview and save stay in sync.
         */
        async uploadFile(file) {
            if (!this.isAdmin || !this.editingKey || !file) return;

            const target = this.sourceStore || this;
            const key = this.editingKey;

            try {
                // 1. Find or create the record
                let record = await pb
                    .collection(target.collection)
                    .getFirstListItem(`key="${key}"`)
                    .catch(() => null);

                if (!record) {
                    record = await pb.collection(target.collection).create({
                        key,
                        // leave url empty for now; we'll fill it after the file lands
                    });
                }

                // 2. Upload the file into the `file` field
                const formData = new FormData();
                formData.append("file", file);

                const updated = await pb
                    .collection(target.collection)
                    .update(record.id, formData);

                // 3. Build the public URL PocketBase serves
                //    (works for both images and videos)
                const filename = updated.file;               // PB stores the filename here
                const publicUrl = pb.files.getUrl(updated, filename);

                // 4. Keep everything in sync
                this.draft = publicUrl;                      // live preview + textarea
                target.items[key] = publicUrl;               // reactive store

                // 5. Also persist the url field so future loads are correct
                await pb.collection(target.collection).update(updated.id, {
                    url: publicUrl,
                });

                console.log(`[${target.collection}] Uploaded file for ${key} → ${publicUrl}`);
            } catch (err) {
                console.error("Upload failed", err);
                alert("Upload failed — check console / PB file rules");
            }
        },

        // Your modal calls .cancel() — keep both names
        cancel() {
            this.modalOpen = false;
        },
        cancelEdit() {
            this.modalOpen = false;
        },

        render(value) {
            return renderer(value ?? "");
        },

        // Used by the modal preview and by the directive
        renderMarkdown(value) {
            return renderer(value ?? "");
        },

        // ---------- Auth ----------
        async login(email, password) {
            try {
                await pb.collection("users").authWithPassword(email, password);
                console.log("Login successful:", this.currentUser?.email);
                await this.init({force: true});
                return {success: true};
            } catch (err) {
                console.error("Login failed", err);
                return {success: false, error: err.message || "Login failed"};
            }
        },

        logout() {
            pb.authStore.clear();
            this.modalOpen = false;
            this.editingKey = "";
            this.draft = "";
            // Keep items so public content stays visible
            console.log(`[${collection}] Logged out`);
        },
    };
}
