import createEditableStore from "./editable-store.js";

export default () =>
    createEditableStore({
        collection: "images",
        field: "src"
    });