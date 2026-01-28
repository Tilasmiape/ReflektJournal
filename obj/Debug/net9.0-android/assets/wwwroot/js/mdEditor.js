window.mdEditor = {
  wrap: function (el, before, after) {
    if (!el) return;
    const start = el.selectionStart ?? 0;
    const end = el.selectionEnd ?? 0;
    const value = el.value ?? "";
    const selected = value.substring(start, end);
    const next = value.substring(0, start) + before + selected + after + value.substring(end);
    el.value = next;
    el.focus();
    const cursor = start + before.length + selected.length + after.length;
    el.setSelectionRange(cursor, cursor);
    el.dispatchEvent(new Event("input"));
  },

  prefixLines: function (el, prefix) {
    if (!el) return;
    const start = el.selectionStart ?? 0;
    const end = el.selectionEnd ?? 0;
    const value = el.value ?? "";
    const sel = value.substring(start, end) || "";
    const lines = (sel.length ? sel : value).split("\n");
    const prefixed = lines.map(l => prefix + l).join("\n");

    let next;
    if (sel.length) {
      next = value.substring(0, start) + prefixed + value.substring(end);
      el.value = next;
      el.focus();
      el.setSelectionRange(start, start + prefixed.length);
    } else {
      el.value = prefixed;
      el.focus();
      el.setSelectionRange(0, prefixed.length);
    }

    el.dispatchEvent(new Event("input"));
  },

  insertAtCursor: function (el, text) {
    if (!el) return;
    const start = el.selectionStart ?? 0;
    const end = el.selectionEnd ?? 0;
    const value = el.value ?? "";
    el.value = value.substring(0, start) + text + value.substring(end);
    el.focus();
    const cursor = start + text.length;
    el.setSelectionRange(cursor, cursor);
    el.dispatchEvent(new Event("input"));
  }
};
