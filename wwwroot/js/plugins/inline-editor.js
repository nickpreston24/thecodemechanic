// src/components/plugins/inline-editor.js
document.addEventListener('alpine:init', () => {
  Alpine.directive('edit', (el, { expression, modifiers }, { cleanup }) => {
    if (!expression) return;

    let key = expression;
    let smartMode = false;

    // Detect smart mode: x-edit="content.something"
    if (expression.startsWith('content.')) {
      key = expression.replace('content.', '');
      smartMode = true;
    }

    // === Smart Mode: Auto-wire reactive content + seeding ===
    if (smartMode) {
      const store = Alpine.store('content');

      if (store?.isAdmin && !store.getContent(key) && el.innerHTML.trim()) {
        store.seedContent(key, el.innerHTML);
      }

      const updateContent = () => {
        const s = Alpine.store('content');
        if (!s) return;
        el.innerHTML = s.renderMarkdown(s.getContent(key));
      };

      const effect = Alpine.effect(updateContent);
      cleanup(() => effect());
    }

   

    // === Always create the edit button ===
    const button = document.createElement('button');
    button.innerHTML = `
      <span class="inline-flex items-center justify-center w-5 h-5 rounded transition-all hover:bg-white/10">
        <svg xmlns="http://www.w3.org/2000/svg" class="w-3.5 h-3.5" fill="none" viewBox="0 0 24 24" stroke="#64748b" stroke-width="2.5">
          <path stroke-linecap="round" stroke-linejoin="round" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
        </svg>
      </span>
    `;
    button.style.cssText = 'margin-left: 6px; vertical-align: middle; opacity: 0; transition: opacity 0.15s ease; display: none;';
    button.title = 'Edit this content';

    // Hover behavior
    const show = () => button.style.opacity = '0.85';
    const hide = () => button.style.opacity = '0';

    el.addEventListener('mouseenter', show);
    el.addEventListener('mouseleave', hide);
    button.addEventListener('mouseenter', show);
    button.addEventListener('mouseleave', hide);

    // Reactively show/hide based on admin
    const updateVisibility = () => {
      const store = Alpine.store('content');
      button.style.display = (store?.isAdmin) ? 'inline' : 'none';
    };

    const visibilityEffect = Alpine.effect(updateVisibility);

    // Click handler
    button.addEventListener('click', (e) => {
      e.preventDefault();
      e.stopImmediatePropagation();
      const store = Alpine.store('content');
      if (store?.startEditing) store.startEditing(key);
    });

    // Insert button
    if (el.nextSibling) {
      el.parentNode.insertBefore(button, el.nextSibling);
    } else {
      el.parentNode.appendChild(button);
    }

    updateVisibility();

    cleanup(() => {
      button.remove();
      el.removeEventListener('mouseenter', show);
      el.removeEventListener('mouseleave', hide);
      visibilityEffect();
    });
  });
});
