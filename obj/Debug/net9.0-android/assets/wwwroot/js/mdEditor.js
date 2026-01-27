
window.mdEditor = {
    wrap: (textarea, before, after) => {
        if (!textarea) return;
        const start = textarea.selectionStart ?? 0;
        const end = textarea.selectionEnd ?? 0;
        const value = textarea.value ?? "";

        const selected = value.substring(start, end);
        const next = value.substring(0, start) + before + selected + after + value.substring(end);

        textarea.value = next;

        const cursor = start + before.length + selected.length + after.length;
        textarea.focus();
        textarea.setSelectionRange(cursor, cursor);
        textarea.dispatchEvent(new Event("input", { bubbles: true }));
    },

    prefixLines: (textarea, prefix) => {
        if (!textarea) return;
        const start = textarea.selectionStart ?? 0;
        const end = textarea.selectionEnd ?? 0;
        const value = textarea.value ?? "";

        const lineStart = value.lastIndexOf("\n", start - 1) + 1;
        const lineEnd = value.indexOf("\n", end);
        const realEnd = lineEnd === -1 ? value.length : lineEnd;

        const block = value.substring(lineStart, realEnd);
        const updated = block
            .split("\n")
            .map(l => (l.trim().length ? prefix + l : l))
            .join("\n");

        const next = value.substring(0, lineStart) + updated + value.substring(realEnd);
        textarea.value = next;

        textarea.focus();
        textarea.setSelectionRange(lineStart, lineStart + updated.length);
        textarea.dispatchEvent(new Event("input", { bubbles: true }));
    },

    insertAtCursor: (textarea, text) => {
        if (!textarea) return;
        const start = textarea.selectionStart ?? 0;
        const end = textarea.selectionEnd ?? 0;
        const value = textarea.value ?? "";

        const next = value.substring(0, start) + text + value.substring(end);
        textarea.value = next;

        const cursor = start + text.length;
        textarea.focus();
        textarea.setSelectionRange(cursor, cursor);
        textarea.dispatchEvent(new Event("input", { bubbles: true }));
    }
};
