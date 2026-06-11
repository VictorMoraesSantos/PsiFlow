---
target: src/Web/PsiFlow.Web/src/pages/OnlineSessionPage.tsx
total_score: 21
p0_count: 0
p1_count: 3
---

# Critique: OnlineSessionPage

Design health score: 21/40.

Detector: clean for OnlineSessionPage.tsx and ResourcePage.tsx.

Priority issues:

1. Online session access is modeled as generic room CRUD, not a secure access workflow.
2. Link actions can fall back to demo URLs or hashes as URLs, which is unsafe and unclear.
3. Click tracking includes a manual "Registrar clique" action that can pollute analytics and clinical audit data.
4. The table does not expose patient/time readiness, link status, or next safe action.
5. User-facing copy includes simulated language and does not distinguish session link, default link, and access audit.

Recommended commands:

1. /impeccable harden atendimento online
2. /impeccable shape atendimento online
3. /impeccable clarify atendimento online
