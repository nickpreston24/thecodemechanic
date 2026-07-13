import Alpine from "../vendor/alpinejs/cdn.min.js";
// import { animate } from "animejs";

console.log("alpine loaded? :>> " + !!Alpine);

import createContentStore from "./stores/content-store.js";

// Global plugins / directives
// import "../components/plugins/tooltip.js";
import "./plugins/inline-editor.js"; // ← Make sure this has the hover version
const content_store = Alpine.store("content", createContentStore());

window.Alpine = Alpine;
Alpine.start();

// after Alpine has started
await Alpine.store("content").init();

console.log("After start:");
console.log("window.Alpine === Alpine", window.Alpine === Alpine);
console.log("body marker", document.body._x_marker);
console.log("h2 marker", document.querySelector("h2[x-text]")?._x_marker);
console.log("stores", content_store);

console.log(Alpine.version);

console.log(
    "%c[Northstar] ✅ Alpine + ContentStore registered",
    "color:#22c55e",
);
