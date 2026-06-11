---
target: src/Web/PsiFlow.Web/src/pages/RecordsPage.tsx
total_score: 22
p0_count: 0
p1_count: 3
---

# Critique: RecordsPage

Design health score: 22/40.

Detector: clean for RecordsPage.tsx and ResourcePage.tsx.

Priority issues:

1. Prontuario is modeled as generic CRUD, not a protected clinical record workspace.
2. High-stakes actions are flat and under-explained: autosave, publish, audit, and evolution history sit together without consequence hierarchy.
3. The table does not expose enough clinical context: patient, document type, pending state, last update, and next safe action are not separated.
4. Copy includes prototype language such as simulated actions, which is unsafe in a clinical documentation context.
5. Evolution actions silently fall back to session id 0 or the first session, which can route clinical writes to the wrong context.

Recommended commands:

1. /impeccable harden prontuario
2. /impeccable shape prontuario
3. /impeccable clarify prontuario
