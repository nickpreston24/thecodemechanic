import createEditableStore from "./editable-store.js?v=3";

export default () =>
    createEditableStore({
        collection: "images",
        field: "url"
    });