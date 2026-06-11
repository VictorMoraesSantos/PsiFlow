---
target: src/Web/PsiFlow.Web/src/pages/SessionsPage.tsx
total_score: 26
p0_count: 0
p1_count: 2
---

# Critique: SessionsPage

Design health score: 26/40.

Detector: clean for SessionsPage.tsx and ResourcePage.tsx.

Priority issues:

1. The Sessions screen is too generic for clinical work. It inherits ResourcePage language and structure, so high-stakes session actions look like CRUD records.
2. Action hierarchy is unsafe. Start, complete, no-show, payment, receipt, and cancel all live as peer drawer actions without state gating or enough consequence framing.
3. Context is missing from the primary table. The row does not show time, patient context, appointment link, or next clinical action clearly enough.
4. Empty and system copy is generic. It says records and operational data where it should teach the session workflow.
5. Date/time formatting is inconsistent. Detail fields show raw datetime strings for sessions.

Recommended commands:

1. /impeccable shape sessoes
2. /impeccable clarify sessoes
3. /impeccable polish sessoes
