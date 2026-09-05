"use strict";

const ui = {
  th: {
    skipToContent: "ข้ามไปเนื้อหาหลัก",
    chooseLanguage: "เลือกภาษา",
    diagramNav: "แผนผังเอกสาร",
    flowFilters: "ตัวกรองแผนผัง",
    flowSummary: "สรุปแผนผัง",
    docsOnly: "เอกสารเท่านั้น · ยังไม่มีโค้ด ERP",
    viewFlows: "ดูแผนผัง",
    readHandbook: "เปิดคู่มือทั้งหมด",
    illustrationCaption: "ภาพประกอบเปลี่ยนตาม Flow ที่เลือก",
    interactiveMap: "INTERACTIVE MAP · แผนผังโต้ตอบได้",
    chooseFlow: "เลือกเรื่องที่ต้องการทำความเข้าใจ",
    jsonNote: "ข้อมูลทุกกล่องและเส้นเชื่อมมาจาก JSON",
    searchLabel: "ค้นหาใน Flow",
    phaseLabel: "ระยะพัฒนา",
    allPhases: "ทุกระยะ",
    steps: "ขั้นตอน",
    connections: "จุดเชื่อม",
    schemaVersion: "Schema",
    diagramHelp: "เลือกกล่องเพื่ออ่านคำอธิบายและเปิดเอกสารที่เกี่ยวข้อง",
    noResults: "ไม่พบขั้นตอนที่ตรงกับตัวกรอง",
    clearFilters: "ล้างตัวกรอง",
    textAlternative: "ดู Flow แบบข้อความและตาราง",
    sequence: "ลำดับขั้นตอน",
    connectionRules: "เงื่อนไขการเชื่อม",
    from: "จาก",
    condition: "เงื่อนไข",
    to: "ไปยัง",
    sourceOfTruth: "รายละเอียดฉบับเต็มอยู่ใน Markdown ซึ่งเป็นแหล่งอ้างอิงหลัก",
    openDocument: "เปิดเอกสารหลัก →",
    openRelatedDocument: "เปิดเอกสารที่เกี่ยวข้อง",
    principlesTitle: "รากฐานที่ยืดหยุ่น แต่ไม่ปล่อยให้คลุมเครือ",
    principleErp: "เริ่มจากการประเมินราคา แล้วต่อยอดเป็นโครงการ จัดซื้อ คลัง ผลิต และ MRP",
    principleTyped: "API, Error และ Query มีสัญญาชัดเจน ลดการเดาและลด Bug",
    principleSecure: "Backend ตรวจ Role, Permission และขอบเขต Resource ทุกครั้ง",
    principleTrace: "Revision, Approval และ Audit ทำให้ตามเหตุผลย้อนหลังได้",
    footerText: "รากฐานเอกสารสำหรับ Project ERP ระดับ Production",
    documentIndex: "ดัชนีเอกสาร",
    loading: "กำลังโหลดแผนผัง…",
    status: { accepted: "อนุมัติแล้ว", draft: "รอยืนยันธุรกิจ", future: "อนาคต" },
    flowLabel: "แผนผัง",
    openStep: "เปิดรายละเอียดขั้นตอน",
    group: "กลุ่ม",
    phase: "ระยะ",
    loadError: "เปิดข้อมูล JSON ไม่สำเร็จ\nกรุณาเปิดโฟลเดอร์นี้ผ่าน Local HTTP Server ตามวิธีใน docs/portal/README.md"
  },
  en: {
    skipToContent: "Skip to main content",
    chooseLanguage: "Choose language",
    diagramNav: "Documentation diagrams",
    flowFilters: "Diagram filters",
    flowSummary: "Diagram summary",
    docsOnly: "DOCUMENTS ONLY · NO ERP CODE",
    viewFlows: "View diagrams",
    readHandbook: "Open the handbook",
    illustrationCaption: "The illustration changes with the selected flow",
    interactiveMap: "INTERACTIVE MAP",
    chooseFlow: "Choose what you want to understand",
    jsonNote: "Every card and connection comes from JSON",
    searchLabel: "Search this flow",
    phaseLabel: "Delivery phase",
    allPhases: "All phases",
    steps: "Steps",
    connections: "Connections",
    schemaVersion: "Schema",
    diagramHelp: "Select a card to read its explanation and open the related document",
    noResults: "No steps match the current filters",
    clearFilters: "Clear filters",
    textAlternative: "View the flow as text and a table",
    sequence: "Step sequence",
    connectionRules: "Connection conditions",
    from: "From",
    condition: "Condition",
    to: "To",
    sourceOfTruth: "The complete Markdown document remains the source of truth",
    openDocument: "Open source document →",
    openRelatedDocument: "Open related document",
    principlesTitle: "Flexible foundations without ambiguity",
    principleErp: "Start with estimation, then extend into projects, purchasing, inventory, production and MRP.",
    principleTyped: "APIs, errors and queries use explicit contracts to reduce guesswork and defects.",
    principleSecure: "The backend checks role, permission and resource scope on every request.",
    principleTrace: "Revisions, approvals and audit records preserve the reason behind every important change.",
    footerText: "Documentation foundation for a production-ready Project ERP.",
    documentIndex: "Document index",
    loading: "Loading diagram…",
    status: { accepted: "Accepted", draft: "Needs business validation", future: "Future" },
    flowLabel: "Flow",
    openStep: "Open step details",
    group: "Group",
    phase: "Phase",
    loadError: "The JSON data could not be loaded.\nServe this folder over local HTTP using the instructions in docs/portal/README.md."
  }
};

const iconGlyphs = {
  customer: "◎",
  survey: "⌖",
  estimate: "Σ",
  approval: "✓",
  project: "▦",
  architecture: "⬡",
  shield: "◇"
};

const state = {
  catalog: null,
  flow: null,
  locale: localStorage.getItem("tanErpDocsLocale") || "th",
  activeFlowId: "business",
  phase: "all",
  search: ""
};

const element = (id) => document.getElementById(id);
const localize = (value) => value?.[state.locale] || value?.th || value?.en || "";

function clearChildren(target) {
  while (target.firstChild) target.removeChild(target.firstChild);
}

function create(tag, options = {}) {
  const node = document.createElement(tag);
  if (options.className) node.className = options.className;
  if (options.text !== undefined) node.textContent = options.text;
  if (options.attrs) {
    Object.entries(options.attrs).forEach(([key, value]) => node.setAttribute(key, value));
  }
  return node;
}

function updateStaticCopy() {
  document.documentElement.lang = state.locale;
  document.querySelectorAll("[data-i18n]").forEach((node) => {
    const key = node.dataset.i18n;
    if (ui[state.locale][key]) node.textContent = ui[state.locale][key];
  });
  document.querySelectorAll("[data-i18n-aria]").forEach((node) => {
    const key = node.dataset.i18nAria;
    if (ui[state.locale][key]) node.setAttribute("aria-label", ui[state.locale][key]);
  });
  document.querySelectorAll("[data-locale]").forEach((button) => {
    button.setAttribute("aria-pressed", String(button.dataset.locale === state.locale));
  });
  element("flow-search").placeholder = state.locale === "th"
    ? "เช่น อนุมัติ, RBAC, Estimate"
    : "For example approval, RBAC, estimate";
  element("close-dialog").setAttribute("aria-label", state.locale === "th" ? "ปิด" : "Close");
}

function renderCatalog() {
  const { catalog } = state;
  element("product-eyebrow").textContent = localize(catalog.product.eyebrow);
  element("hero-title").textContent = localize(catalog.product.title);
  element("product-description").textContent = localize(catalog.product.description);

  const tabs = element("flow-tabs");
  clearChildren(tabs);
  catalog.flows.forEach((flow, index) => {
    const button = create("button", {
      attrs: {
        type: "button",
        role: "tab",
        id: `tab-${flow.id}`,
        "aria-controls": "flow-panel",
        "aria-selected": String(flow.id === state.activeFlowId),
        tabindex: flow.id === state.activeFlowId ? "0" : "-1"
      }
    });
    button.append(create("span", { className: "tab-number", text: `${String(index + 1).padStart(2, "0")} / ${String(catalog.flows.length).padStart(2, "0")}` }));
    button.append(create("span", { text: localize(flow.shortTitle) }));
    button.addEventListener("click", () => selectFlow(flow.id));
    button.addEventListener("keydown", (event) => {
      const supportedKeys = ["ArrowRight", "ArrowLeft", "Home", "End"];
      if (!supportedKeys.includes(event.key)) return;
      event.preventDefault();
      let nextIndex = index;
      if (event.key === "ArrowRight") nextIndex = (index + 1) % catalog.flows.length;
      if (event.key === "ArrowLeft") nextIndex = (index - 1 + catalog.flows.length) % catalog.flows.length;
      if (event.key === "Home") nextIndex = 0;
      if (event.key === "End") nextIndex = catalog.flows.length - 1;
      const nextFlow = catalog.flows[nextIndex];
      selectFlow(nextFlow.id).then(() => element(`tab-${nextFlow.id}`)?.focus());
    });
    tabs.append(button);
  });

  const phaseSelect = element("phase-filter");
  const current = phaseSelect.value || state.phase;
  clearChildren(phaseSelect);
  phaseSelect.append(create("option", { text: ui[state.locale].allPhases, attrs: { value: "all" } }));
  catalog.phases.forEach((phase) => {
    phaseSelect.append(create("option", { text: localize(phase.label), attrs: { value: phase.id } }));
  });
  phaseSelect.value = current;
}

function renderFlow() {
  const flow = state.flow;
  if (!flow) return;

  const flowIndex = state.catalog.flows.findIndex((item) => item.id === flow.id) + 1;
  element("flow-index").textContent = `${ui[state.locale].flowLabel.toUpperCase()} ${String(flowIndex).padStart(2, "0")}`;
  element("flow-panel").setAttribute("aria-labelledby", `tab-${flow.id}`);
  element("flow-title").textContent = localize(flow.title);
  element("flow-description").textContent = localize(flow.description);
  element("schema-version").textContent = `v${flow.version}`;
  element("flow-document").href = flow.document || "../README.md";
  if (flow.illustration) {
    element("hero-illustration").src = flow.illustration.src;
    element("hero-illustration").alt = localize(flow.illustration.alt);
  }

  const query = state.search.trim().toLocaleLowerCase(state.locale);
  const nodes = [...flow.nodes]
    .sort((a, b) => a.order - b.order)
    .filter((node) => state.phase === "all" || node.phase === state.phase)
    .filter((node) => !query || `${localize(node.title)} ${localize(node.summary)} ${node.id}`.toLocaleLowerCase(state.locale).includes(query));
  const visibleIds = new Set(nodes.map((node) => node.id));
  const edges = flow.edges.filter((edge) => visibleIds.has(edge.from) && visibleIds.has(edge.to));

  element("node-count").textContent = String(nodes.length);
  element("edge-count").textContent = String(edges.length);
  renderLegend(flow, nodes);
  renderCards(flow, nodes);
  renderTextAlternative(flow, nodes, edges);
  element("empty-state").hidden = nodes.length > 0;
}

function renderLegend(flow, nodes) {
  const legend = element("group-legend");
  clearChildren(legend);
  const groups = new Set(nodes.map((node) => node.group));
  flow.groups.filter((group) => groups.has(group.id)).forEach((group) => {
    legend.append(create("span", { className: "legend-item", text: localize(group.label) }));
  });
}

function renderCards(flow, nodes) {
  const grid = element("flow-grid");
  clearChildren(grid);
  const groups = new Map(flow.groups.map((group) => [group.id, group]));

  nodes.forEach((node, index) => {
    const item = create("li", { className: "flow-card-wrap" });
    const button = create("button", {
      className: "flow-card",
      attrs: { type: "button", "aria-label": `${ui[state.locale].openStep}: ${localize(node.title)}` }
    });
    const head = create("span", { className: "flow-card-head" });
    head.append(create("span", { className: "node-icon", text: iconGlyphs[node.icon] || "•", attrs: { "aria-hidden": "true" } }));
    head.append(create("span", { className: "step-number", text: `STEP ${String(index + 1).padStart(2, "0")}` }));
    button.append(head);
    button.append(create("h3", { text: localize(node.title) }));
    button.append(create("p", { text: localize(node.summary) }));

    const meta = create("span", { className: "card-meta" });
    meta.append(create("span", { className: "tag", text: localize(groups.get(node.group)?.label) }));
    meta.append(create("span", { className: `tag tag-status-${node.status}`, text: ui[state.locale].status[node.status] }));
    button.append(meta);
    button.addEventListener("click", () => openNodeDialog(node, groups.get(node.group)));
    item.append(button);
    grid.append(item);
  });
}

function renderTextAlternative(flow, nodes, edges) {
  const sequence = element("sequence-list");
  clearChildren(sequence);
  nodes.forEach((node) => {
    const item = create("li");
    item.append(create("strong", { text: localize(node.title) }));
    item.append(document.createTextNode(` — ${localize(node.summary)}`));
    sequence.append(item);
  });

  const nodeById = new Map(flow.nodes.map((node) => [node.id, node]));
  const table = element("connection-table");
  clearChildren(table);
  edges.forEach((edge) => {
    const row = create("tr");
    row.append(create("td", { text: localize(nodeById.get(edge.from)?.title) }));
    row.append(create("td", { text: localize(edge.label) }));
    row.append(create("td", { text: localize(nodeById.get(edge.to)?.title) }));
    table.append(row);
  });
}

function openNodeDialog(node, group) {
  element("dialog-icon").textContent = iconGlyphs[node.icon] || "•";
  element("dialog-title").textContent = localize(node.title);
  element("dialog-summary").textContent = localize(node.summary);
  element("dialog-meta").textContent = `${ui[state.locale].group}: ${localize(group?.label)} · ${ui[state.locale].phase}: ${phaseLabel(node.phase)} · ${ui[state.locale].status[node.status]}`;
  const details = element("dialog-details");
  clearChildren(details);
  (node.details || []).forEach((detail) => details.append(create("li", { text: localize(detail) })));
  details.hidden = !node.details?.length;
  element("dialog-document").href = node.document || state.flow.document || "../README.md";
  element("node-dialog").showModal();
}

function phaseLabel(phaseId) {
  const phase = state.catalog.phases.find((item) => item.id === phaseId);
  return phase ? localize(phase.label) : phaseId;
}

async function selectFlow(flowId) {
  const descriptor = state.catalog.flows.find((flow) => flow.id === flowId) || state.catalog.flows[0];
  state.activeFlowId = descriptor.id;
  history.replaceState(null, "", `#${descriptor.id}`);
  element("loading-state").hidden = false;
  element("flow-panel").hidden = true;
  element("error-state").hidden = true;
  try {
    const response = await fetch(`data/${descriptor.file}`);
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    state.flow = await response.json();
    state.search = "";
    state.phase = "all";
    element("flow-search").value = "";
    element("phase-filter").value = "all";
    renderCatalog();
    renderFlow();
    element("flow-panel").hidden = false;
  } catch (error) {
    element("error-state").textContent = `${ui[state.locale].loadError}\n(${error.message})`;
    element("error-state").hidden = false;
  } finally {
    element("loading-state").hidden = true;
  }
}

async function initialize() {
  updateStaticCopy();
  element("loading-state").textContent = ui[state.locale].loading;
  try {
    const response = await fetch("data/portal.json");
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    state.catalog = await response.json();
    if (!state.catalog.locales.includes(state.locale)) state.locale = state.catalog.defaultLocale;
    const requestedFlow = location.hash.replace("#", "");
    state.activeFlowId = state.catalog.flows.some((flow) => flow.id === requestedFlow) ? requestedFlow : state.catalog.flows[0].id;
    updateStaticCopy();
    renderCatalog();
    await selectFlow(state.activeFlowId);
  } catch (error) {
    element("loading-state").hidden = true;
    element("error-state").textContent = `${ui[state.locale].loadError}\n(${error.message})`;
    element("error-state").hidden = false;
  }
}

document.querySelectorAll("[data-locale]").forEach((button) => {
  button.addEventListener("click", () => {
    state.locale = button.dataset.locale;
    localStorage.setItem("tanErpDocsLocale", state.locale);
    updateStaticCopy();
    if (state.catalog) renderCatalog();
    if (state.flow) renderFlow();
  });
});

element("flow-search").addEventListener("input", (event) => {
  state.search = event.target.value;
  renderFlow();
});

element("phase-filter").addEventListener("change", (event) => {
  state.phase = event.target.value;
  renderFlow();
});

element("clear-filters").addEventListener("click", () => {
  state.search = "";
  state.phase = "all";
  element("flow-search").value = "";
  element("phase-filter").value = "all";
  renderFlow();
  element("flow-search").focus();
});

element("close-dialog").addEventListener("click", () => element("node-dialog").close());
element("node-dialog").addEventListener("click", (event) => {
  if (event.target === element("node-dialog")) element("node-dialog").close();
});

window.addEventListener("hashchange", () => {
  const requestedFlow = location.hash.replace("#", "");
  if (state.catalog?.flows.some((flow) => flow.id === requestedFlow) && requestedFlow !== state.activeFlowId) selectFlow(requestedFlow);
});

initialize();
