// site.js — order is extremely important
import Alpine from "/vendor/alpinejs/module.esm.min.js";
import createContentStore from "./stores/content-store.js?v=3";
import createImageStore from "./stores/image-store.js?v=3";
import createVideoStore from "./stores/video-store.js?v=3";

// 1. Expose Alpine globally immediately
window.Alpine = Alpine;

// 2. Register stores BEFORE any await and BEFORE Alpine.start()
//    The edit sidebar already contains $store.content.* expressions.
//    If these are not present when Alpine starts walking the DOM → "Cannot read properties of undefined"
Alpine.store("content", createContentStore());
Alpine.store("images", createImageStore());
Alpine.store("videos", createVideoStore());

// 3. Register the custom directive (listens for alpine:init)
await import("./plugins/inline-editor.js");

// 4. Load data (404s are expected until you create the collections)
await Promise.allSettled([
    Alpine.store("content").init(),
    Alpine.store("images").init(),
    Alpine.store("videos").init(),
]);

// 5. Start Alpine last
Alpine.start();

console.log(
    "%c[CodeMechanic] ✅ Alpine + Content/Images/Videos stores registered",
    "color:#22c55e"
);
console.log("Alpine version:", Alpine.version);
console.log("site.js loaded! <3");

// ✅ Correct for Anime.js v4 UMD
// The global object is still 'anime', but the function is now 'animate'

// Option A: If 'animate' is exposed globally (common in some UMD builds)
if (typeof animate !== 'undefined') {
    // animate('.box', {translateX: 250});
    console.log("use 'animate()'")
}
// Option B: If only 'anime' object exists (most likely for v4 UMD)
else if (typeof anime !== 'undefined') {
    // In v4, the main function is 'animate' inside the 'anime' object
    // anime.animate('.box', {translateX: 250});
    console.log("use 'anime()'")
} else {
    console.error("Anime.js not loaded");
}



