// contentStore.js
console.log("loading content-store.js ...")

// import PocketBase from "pocketbase";
import PocketBase from "../../vendor/pocketbase/pocketbase.es.mjs";

export default function createContentStore() {

    console.log("pb url (injected from _Layout) :>>  " + window.AppConfig.pocketbaseUrl)
    const pb = new PocketBase(window.AppConfig.pocketbaseUrl ?? "https://pocketbase-railway-production-cc55.up.railway.app");

    pb.autoCancellation(false);

    return {
        contents: {},
        editingKey: null,
        draft: "",
        modalOpen: false,

        get isAuthenticated() {
            return pb.authStore.isValid;
        },

        get isAdmin() {

            console.log('role? :>> ' + pb.authStore.record?.role)
            return this.isAuthenticated && pb.authStore.record?.role === "admin";
        },

        get currentUser() {
            return pb.authStore.record;
        },

        async init() {
            console.log("%c[DEBUG] ContentStore.init() called", "color: #eab308");
            if (this.isAuthenticated && this.isAdmin) {
                await this.loadAll();
            }
        },

        async login(email, password) {
            try {
                await pb.collection("users").authWithPassword(email, password);
                console.log("Login successful:", this.currentUser?.email);
                await this.loadAll();
                return {success: true};
            } catch (err) {
                console.error("Login failed", err);
                return {success: false, error: err.message || "Login failed"};
            }
        },

        logout() {
            pb.authStore.clear();
            this.contents = {};
            this.modalOpen = false;
            this.editingKey = null;
            this.draft = "";
        },

        async loadAll() {
            try {

                if (!this.isAdmin) {
                    console.warn("not in admin mode.")
                    return;
                }

                const records = await pb
                    .collection("editable_content")
                    .getFullList({sort: "-updated"});

                // Clear and repopulate (preserves reactivity)
                Object.keys(this.contents).forEach((k) => delete this.contents[k]);
                records.forEach((record) => {
                    this.contents[record.key] = record;
                });

                console.log(
                    `Loaded ${Object.keys(this.contents).length} editable items`,
                );
            } catch (err) {

                console.error("Failed to load editable_content", err);

                console.log("isAdmin:", this.isAdmin);
                console.log("currentUser role:", this.currentUser?.role);

            }
        },

        async seedContent(key, content) {
            if (!key || !content?.trim()) return;

            try {
                // Check if this key already exists in the database
                const existing = await pb
                    .collection("editable_content")
                    .getFirstListItem(`key="${key}"`)
                    .catch(() => null);

                if (existing) {
                    // Key already exists — just cache it locally and exit
                    this.contents[key] = existing;
                    return;
                }

                // Key doesn't exist — create it
                const created = await pb.collection("editable_content").create({
                    key,
                    content: content.trim(),
                    content_type: "markdown",
                });

                this.contents[key] = created;
                console.log(`[ContentStore] Seeded new key: ${key}`);

            } catch (err) {
                console.error(`Failed to seed content for key "${key}"`, err);
            }
        },

        getContent(key) {
            console.log("grabbing content for key :>> " + key);
            let content = this.contents[key]?.content || "";
            console.log("content :>> " + content);
            return content;
        },

        renderMarkdown(md) {
            if (!md) return "";
            // Add your regex vetting here
            return md
                .replace(/&/g, "&amp;")
                .replace(/</g, "&lt;")
                .replace(/>/g, "&gt;")
                .replace(/\*\*(.+?)\*\*/g, "<strong>$1</strong>")
                .replace(/\*(.+?)\*/g, "<em>$1</em>")
                .replace(/\n/g, "<br>");
        },

        async startEditing(key) {
            console.log("editing key :>> " + key);

            console.log("isAdmin:", this.isAdmin);
            console.log("currentUser role:", this.currentUser?.role);

            if (!this.isAdmin) {
                alert("Admin access required");
                return;
            }
            this.editingKey = key;
            this.draft = this.getContent(key);
            this.modalOpen = true;
            console.log("finished editing key :>> " + key);

            console.log("this.modalOpen :>> " + this.modalOpen);
        },

        async saveEdit() {
            if (!this.isAdmin || !this.editingKey) return;

            const key = this.editingKey;
            const newContent = this.draft.trim();

            try {
                const existing = this.contents[key];
                if (existing?.id) {
                    const updated = await pb
                        .collection("editable_content")
                        .update(existing.id, {
                            content: newContent,
                        });
                    this.contents[key] = updated;
                } else {
                    const created = await pb.collection("editable_content").create({
                        key,
                        content: newContent,
                        content_type: "markdown",
                    });
                    this.contents[key] = created;
                }

                this.modalOpen = false;
                this.editingKey = null;
                this.draft = "";
                console.log(`Saved: ${key}`);
            } catch (err) {
                console.error("Save failed", err);
                alert("Save failed");
            }
        },

        cancelEdit() {
            this.modalOpen = false;
            this.editingKey = null;
            this.draft = "";
        },
    };
}
