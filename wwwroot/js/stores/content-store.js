// import { marked } from "marked";
import createEditableStore from "./editable-store.js";

// Simple, XSS-safe markdown renderer (same spirit as the working legacy store).
// Replace with marked.parse later if you want full Markdown + GFM.
function simpleMarkdown(md) {
    if (!md) return "";
    return md
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/\*\*(.+?)\*\*/g, "<strong>$1</strong>")
        .replace(/\*(.+?)\*/g, "<em>$1</em>")
        .replace(/\n/g, "<br>");
}

export default () =>
    createEditableStore({
        collection: "editable_content",
        field: "content",
        renderer: simpleMarkdown,
        // renderer: marked.parse   // uncomment + import if you add marked
    });
