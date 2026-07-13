// import Alpine from "alpinejs";
import Alpine from "/vendor/alpinejs/module.esm.min.js";
import createContentStore from "./stores/content-store.js";

window.Alpine = Alpine;

await import("./plugins/inline-editor.js");

Alpine.store("content", createContentStore());

await Alpine.store("content").init();

Alpine.start();

// import createContentStore from "./stores/content-store.js";
//
// // Global plugins / directives
// import "./plugins/inline-editor.js"; // ← Make sure this has the hover version
// // const content_store = Alpine.store("content", createContentStore());
//
// console.log("%c[DEBUG] window.Alpine? ::>> " + !!window.Alpine, "color: #eab308");
//
//
// window.Alpine = Alpine;
// console.log("inline editor loaded", window.Alpine);
//
// Alpine.store("content", createContentStore());
//
// await Alpine.store("content").init();
//
// Alpine.start();
//
//
// // Alpine.start();
// //
// // // after Alpine has started
// // await Alpine.store("content").init();
// // Alpine.initTree(document.body);
//
// //
console.log("After start:");
console.log("window.Alpine === Alpine", window.Alpine === Alpine);
console.log("body marker", document.body._x_marker);
console.log("h2 marker", document.querySelector("h2[x-text]")?._x_marker);
// console.log("stores", content_store);

console.log(Alpine.version);

console.log(
    "%c[CodeMechanic] ✅ Alpine + ContentStore registered",
    "color:#22c55e",
);


console.log("site.js loaded! <3")

//
//
