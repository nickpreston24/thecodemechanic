// src/components/plugins/inline-editor.js
// Magic inline editor for text / img / video
// x-edit="key"  or  x-edit="content.key" / "images.key" / "videos.key"
//
// This version completely avoids the Maximum call stack size exceeded error
// by never letting the reactive effect mutate the host element in a way
// that re-triggers Alpine's directive system.

console.log("loading inline-editor.js...");

document.addEventListener("alpine:init", () => {
    Alpine.directive("edit", (el, {expression}, {effect, cleanup, evaluateLater}) => {
        if (!expression || el._x_edit_bound) return;
        el._x_edit_bound = true; // hard guard against re-entrant directive evaluation

        const tag = el.tagName.toLowerCase();

        // ---------- resolve store + key ----------
        let storeName = "content";
        let key = expression;

        if (expression.startsWith("content.")) {
            key = expression.slice("content.".length);
            storeName = "content";
        } else if (expression.startsWith("images.") || expression.startsWith("image.")) {
            key = expression.replace(/^images?\./, "");
            storeName = "images";
        } else if (expression.startsWith("videos.") || expression.startsWith("video.")) {
            key = expression.replace(/^videos?\./, "");
            storeName = "videos";
        }

        if (storeName === "content") {
            if (tag === "img") storeName = "images";
            if (tag === "video") storeName = "videos";
        }

        console.log(`[x-edit] store=${storeName} key=${key} tag=${tag}`);

        const getStore = () => Alpine.store(storeName);

        // Snapshot original content ONCE (before any CMS value is applied)
        if (tag !== "img" && tag !== "video") {
            el._x_edit_original = el.innerHTML.trim();
        } else {
            el._x_edit_original =
                el.getAttribute("src") ||
                el.querySelector("source")?.getAttribute("src") ||
                "";
        }

        // ---------- pure getters / setters (no Alpine tracking inside) ----------
        const readCurrent = () => {
            if (tag === "img") return el.getAttribute("src") || "";
            if (tag === "video") {
                return (
                    el.getAttribute("src") ||
                    el.querySelector("source")?.getAttribute("src") ||
                    ""
                );
            }
            return el.innerHTML.trim();
        };

        const applyValue = (value) => {
            if ((tag === "img" || tag === "video") && String(value).trim() === "") return;

            if (tag === "img") {
                if (el.getAttribute("src") !== String(value)) {
                    el.setAttribute("src", value);
                }
                return;
            }

            if (tag === "video") {
                const source = el.querySelector("source");
                const current =
                    el.getAttribute("src") || source?.getAttribute("src") || "";
                if (current === String(value)) return;

                if (source) {
                    source.setAttribute("src", value);
                } else {
                    el.setAttribute("src", value);
                }
                try {
                    el.load();
                } catch (_) {
                }
                return;
            }

            // text content
            const s = getStore();
            if (!s) return;
            // const rendered = (s.renderMarkdown || s.render || ((x) => x))(value);
            const rendered = s.renderMarkdown
                ? s.renderMarkdown(value)
                : (s.render ? s.render(value) : value);

            if (el.innerHTML.trim() === String(rendered).trim()) return;

            // The critical line: we write, but we have already set el._x_edit_bound
            // so even if Alpine re-evaluates the directive it will early-return.
            el.innerHTML = rendered;
        };

        // ---------- seed (admin only) ----------
        const trySeed = () => {
            const s = getStore();
            if (!s?.isAdmin) return;
            const current = el._x_edit_original;
            if (current && s.get(key) === null) {
                s.seed(key, current);
            }
        };
        trySeed();

        // ---------- THE REACTIVE PART ----------
        // We only track the *primitive value* from the store.
        // The actual DOM write is done in a microtask so it happens
        // outside the current effect's dependency tracking.
        // Combined with the _x_edit_bound guard this completely eliminates
        // the call-stack overflow.
        let lastApplied = Symbol("unset");

        effect(() => {
            const s = getStore();
            if (!s) return;

            const val = s.get(key); // this is the only reactive read

            // Bail if nothing changed
            if (val === lastApplied) return;
            lastApplied = val;

            // Defer the mutation so it is not part of the effect's tracking
            queueMicrotask(() => {
                if (val !== null) {
                    applyValue(val);
                }
                // if val === null we intentionally leave the original HTML alone
            });
        });

        // ---------- edit button ----------
        const button = document.createElement("button");
        button.type = "button";
        button.title = "Edit";
        button.innerHTML = `
<span class="inline-flex items-center justify-center w-5 h-5 rounded transition-all hover:bg-white/10">
<svg xmlns="http://www.w3.org/2000/svg" class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="#64748b" stroke-width="2.5">
<path stroke-linecap="round" stroke-linejoin="round" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z"/>
</svg>
</span>`;
        button.style.cssText = `
            margin-left:6px; vertical-align:middle; opacity:0; display:none;
            transition:opacity .15s ease; border:none; background:transparent;
            cursor:pointer; padding:0; line-height:1;
        `;

        const show = () => (button.style.opacity = "0.85");
        const hide = () => (button.style.opacity = "0");

        el.addEventListener("mouseenter", show);
        el.addEventListener("mouseleave", hide);
        button.addEventListener("mouseenter", show);
        button.addEventListener("mouseleave", hide);

        // Button visibility also uses the injected effect (safe — only style change)
        effect(() => {
            button.style.display = getStore()?.isAdmin ? "inline" : "none";
        });

        button.addEventListener("click", (e) => {
            e.preventDefault();
            e.stopImmediatePropagation();

            const realStore = getStore();
            const type = storeName;                 // "content" | "images" | "videos"

            // Always open the content modal (the only one that exists)

            Alpine.store("content").startEditing({
                key,
                type: storeName,          // "content" | "images" | "videos"
                sourceStore: getStore(),  // the real store
            });
            
            // Alpine.store("content").startEditing({
            //     key,
            //     type,
            //     sourceStore: realStore,
            // });
        });

        if (el.nextSibling) {
            el.parentNode.insertBefore(button, el.nextSibling);
        } else {
            el.parentNode.appendChild(button);
        }

        cleanup(() => {
            button.remove();
            el.removeEventListener("mouseenter", show);
            el.removeEventListener("mouseleave", hide);
            delete el._x_edit_bound;
        });
    });
});
