import createEditableStore from "./editable-store.js";

export default () =>
    createEditableStore({
        collection: "videos",
        field: "url"
    });