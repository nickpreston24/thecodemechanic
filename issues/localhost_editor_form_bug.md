VM270 module.esm.min.js:5 Uncaught SyntaxError: Unexpected token 'export' (at VM270 module.esm.min.js:5:39189)
content-store.js:2 loading content-store.js ...
(index):209 [DEBUG] loading content sidebar!
inline-editor.js:2 loading inline-editor.js 3 ...
content-store.js:9 pb url (injected from _Layout) :>>  https://pocketbase-railway-codemechanic.up.railway.app/
content-store.js:35 [DEBUG] ContentStore.init() called
content-store.js:26 role? :>> admin
content-store.js:26 role? :>> admin
content-store.js:35 [DEBUG] ContentStore.init() called
content-store.js:26 role? :>> admin
content-store.js:26 role? :>> admin
content-store.js:79 Loaded 4 editable items
content-store.js:79 Loaded 4 editable items
inline-editor.js:4 inside x-edit directive... 
inline-editor.js:8 inside x-edit directive... 
content-store.js:26 role? :>> admin
content-store.js:124 grabbing content for key :>> cm-tagline
content-store.js:126 content :>> I don't replace developers—I replace repetitive edits.
content-store.js:124 grabbing content for key :>> cm-tagline
content-store.js:126 content :>> I don't replace developers—I replace repetitive edits.
content-store.js:26 role? :>> admin
content-store.js:26 role? :>> admin
site.js?v=CxWjnFl8vLIK0MqGpEU8pofqu2T938aVogudX4cJGz8:41 After start:
site.js?v=CxWjnFl8vLIK0MqGpEU8pofqu2T938aVogudX4cJGz8:42 window.Alpine === Alpine true
site.js?v=CxWjnFl8vLIK0MqGpEU8pofqu2T938aVogudX4cJGz8:43 body marker undefined
site.js?v=CxWjnFl8vLIK0MqGpEU8pofqu2T938aVogudX4cJGz8:44 h2 marker undefined
site.js?v=CxWjnFl8vLIK0MqGpEU8pofqu2T938aVogudX4cJGz8:47 3.13.10
site.js?v=CxWjnFl8vLIK0MqGpEU8pofqu2T938aVogudX4cJGz8:49 [CodeMechanic] ✅ Alpine + ContentStore registered
site.js?v=CxWjnFl8vLIK0MqGpEU8pofqu2T938aVogudX4cJGz8:55 site.js loaded! <3
content-store.js:143 editing key :>> cm-tagline
content-store.js:26 role? :>> admin
content-store.js:145 isAdmin: true
content-store.js:146 currentUser role: admin
content-store.js:26 role? :>> admin
content-store.js:124 grabbing content for key :>> cm-tagline
content-store.js:126 content :>> I don't replace developers—I replace repetitive edits.
content-store.js:155 finished editing key :>> cm-tagline
content-store.js:157 this.modalOpen :>> true


----  ^^^ after a refresh ----


---  first load (localhost) ---- 


Uncaught SyntaxError: Unexpected token 'export'
content-store.js:2 loading content-store.js ...
(index):209 [DEBUG] loading content sidebar!
module.esm.min.js:1 Alpine Expression Error: Cannot read properties of undefined (reading 'modalOpen')

Expression: "$store.content.modalOpen"

 <div x-show=​"$store.content.modalOpen" @keydown.window.escape=​"$store.content.cancel()​" class=​"relative z-[99]​" data-teleport-target=​"true" style=​"display:​ none;​">​…​</div>​
te @ module.esm.min.js:1
module.esm.min.js:1 Alpine Expression Error: Cannot read properties of undefined (reading 'modalOpen')

Expression: "$store.content.modalOpen"

 <div x-show=​"$store.content.modalOpen" x-transition.opacity.duration.300ms @click=​"$store.content.cancel()​" class=​"fixed inset-0 bg-black/​60" style=​"display:​ none;​">​ ​</div>​
te @ module.esm.min.js:1
module.esm.min.js:1 Alpine Expression Error: Cannot read properties of undefined (reading 'modalOpen')

Expression: "$store.content.modalOpen"

 <div x-show=​"$store.content.modalOpen" x-transition:enter=​"transform transition ease-in-out duration-500 sm:​duration-700" x-transition:enter-start=​"translate-x-full" x-transition:enter-end=​"translate-x-0" x-transition:leave=​"transform transition ease-in-out duration-500 sm:​duration-700" x-transition:leave-start=​"translate-x-0" x-transition:leave-end=​"translate-x-full" class=​"w-screen max-w-md" style=​"display:​ none;​">​…​</div>​
te @ module.esm.min.js:1
module.esm.min.js:1 Alpine Expression Error: Cannot read properties of undefined (reading 'editingKey')

Expression: "$store.content.editingKey"

 <p class=​"font-mono text-xs" style=​"color:​ #64748b;​" x-text=​"$store.content.editingKey">​</p>​
te @ module.esm.min.js:1
module.esm.min.js:1 Alpine Expression Error: Cannot read properties of undefined (reading 'draft')

Expression: "$store.content.draft"

<textarea x-model=​"$store.content.draft" rows=​"10" class=​"w-full rounded-md border p-3 font-mono text-sm focus:​outline-none" style=​"background-color:​ #0a0a0a;​ border-color:​ rgba(226, 232, 240, 0.15)​;​ color:​ #e2e8f0;​" placeholder=​"Enter markdown...">​</textarea>​
te @ module.esm.min.js:1
module.esm.min.js:1 Alpine Expression Error: Cannot read properties of undefined (reading 'renderMarkdown')

Expression: "$store.content.renderMarkdown($store.content.draft)"

 <div class=​"prose prose-sm max-w-none min-h-[120px]​ rounded-md border p-4" style=​"background-color:​ #0a0a0a;​ border-color:​ rgba(226, 232, 240, 0.15)​;​ color:​ #e2e8f0;​" x-html=​"$store.content.renderMarkdown($store.content.draft)​">​undefined​</div>​
te @ module.esm.min.js:1
inline-editor.js:2 loading inline-editor.js 3 ...
content-store.js:9 pb url (injected from _Layout) :>>  https://pocketbase-railway-codemechanic.up.railway.app/
content-store.js:35 [DEBUG] ContentStore.init() called
2content-store.js:26 role? :>> admin
content-store.js:35 [DEBUG] ContentStore.init() called
2content-store.js:26 role? :>> admin
3module.esm.min.js:5 Uncaught TypeError: Cannot read properties of undefined (reading 'modalOpen')
    at [Alpine] $store.content.modalOpen (eval at <anonymous> (module.esm.min.js:5:665), <anonymous>:3:47)
    at module.esm.min.js:5:1068
    at rr (module.esm.min.js:1:5076)
    at module.esm.min.js:5:34779
    at r (module.esm.min.js:5:18274)
    at Object.Qr [as effect] (module.esm.min.js:5:18062)
    at D (module.esm.min.js:1:392)
    at module.esm.min.js:1:505
    at Function.<anonymous> (module.esm.min.js:5:34773)
    at r (module.esm.min.js:5:2323)
module.esm.min.js:5 Uncaught TypeError: Cannot read properties of undefined (reading 'editingKey')
    at [Alpine] $store.content.editingKey (eval at <anonymous> (module.esm.min.js:5:665), <anonymous>:3:47)
    at module.esm.min.js:5:1068
    at rr (module.esm.min.js:1:5076)
    at module.esm.min.js:5:32850
    at r (module.esm.min.js:5:18274)
    at Object.Qr [as effect] (module.esm.min.js:5:18062)
    at D (module.esm.min.js:1:392)
    at module.esm.min.js:1:505
    at Function.<anonymous> (module.esm.min.js:5:32843)
    at r (module.esm.min.js:5:2323)
module.esm.min.js:5 Uncaught TypeError: Cannot read properties of undefined (reading 'draft')
    at [Alpine] $store.content.draft (eval at <anonymous> (module.esm.min.js:5:665), <anonymous>:3:47)
    at module.esm.min.js:5:1068
    at rr (module.esm.min.js:1:5076)
    at c (module.esm.min.js:5:30340)
    at module.esm.min.js:5:31326
    at r (module.esm.min.js:5:18274)
    at Object.Qr [as effect] (module.esm.min.js:5:18062)
    at D (module.esm.min.js:1:392)
    at module.esm.min.js:1:505
    at Function.<anonymous> (module.esm.min.js:5:31313)
module.esm.min.js:5 Uncaught TypeError: Cannot read properties of undefined (reading 'renderMarkdown')
    at [Alpine] $store.content.renderMarkdown($store.content.draft) (eval at <anonymous> (module.esm.min.js:5:665), <anonymous>:3:47)
    at module.esm.min.js:5:1068
    at rr (module.esm.min.js:1:5076)
    at module.esm.min.js:5:32962
    at r (module.esm.min.js:5:18274)
    at Object.Qr [as effect] (module.esm.min.js:5:18062)
    at D (module.esm.min.js:1:392)
    at module.esm.min.js:1:505
    at Function.<anonymous> (module.esm.min.js:5:32955)
    at r (module.esm.min.js:5:2323)
2content-store.js:79 Loaded 4 editable items
inline-editor.js:4 inside x-edit directive... 
inline-editor.js:8 inside x-edit directive... 
content-store.js:26 role? :>> admin
content-store.js:124 grabbing content for key :>> cm-tagline
content-store.js:126 content :>> I don't replace developers—I replace repetitive edits.
content-store.js:124 grabbing content for key :>> cm-tagline
content-store.js:126 content :>> I don't replace developers—I replace repetitive edits.
2content-store.js:26 role? :>> admin
site.js?v=CxWjnFl8vLIK0MqGpEU8pofqu2T938aVogudX4cJGz8:41 After start:
site.js?v=CxWjnFl8vLIK0MqGpEU8pofqu2T938aVogudX4cJGz8:42 window.Alpine === Alpine true
site.js?v=CxWjnFl8vLIK0MqGpEU8pofqu2T938aVogudX4cJGz8:43 body marker undefined
site.js?v=CxWjnFl8vLIK0MqGpEU8pofqu2T938aVogudX4cJGz8:44 h2 marker undefined
site.js?v=CxWjnFl8vLIK0MqGpEU8pofqu2T938aVogudX4cJGz8:47 3.13.10
site.js?v=CxWjnFl8vLIK0MqGpEU8pofqu2T938aVogudX4cJGz8:49 [CodeMechanic] ✅ Alpine + ContentStore registered
site.js?v=CxWjnFl8vLIK0MqGpEU8pofqu2T938aVogudX4cJGz8:55 site.js loaded! <3