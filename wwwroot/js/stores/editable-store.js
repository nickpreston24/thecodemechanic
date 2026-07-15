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

            const key = this.editingKey;
            const value = typeof this.draft === "string" ? this.draft.trim() : this.draft;

            try {
                const existing = await pb
                    .collection(collection)
                    .getFirstListItem(`key="${key}"`)
                    .catch(() => null);

                if (existing) {
                    await pb.collection(collection).update(existing.id, {
                        [field]: value,
                    });
                } else {
                    await pb.collection(collection).create({
                        key,
                        [field]: value,
                    });
                }

                this.items[key] = value;
                this.modalOpen = false;
                console.log(`[${collection}] Saved: ${key}`);
            } catch (err) {
                console.error(`[${collection}] Save failed for "${key}"`, err);
                alert("Save failed — check console / PocketBase rules");
            }
        },

        startEditing({ key }) {
            if (!this.isAdmin) {
                alert("Admin access required");
                return;
            }
            this.editingKey = key;
            this.draft = this.get(key) ?? "";
            this.modalOpen = true;
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
                await this.init();
                return { success: true };
            } catch (err) {
                console.error("Login failed", err);
                return { success: false, error: err.message || "Login failed" };
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
