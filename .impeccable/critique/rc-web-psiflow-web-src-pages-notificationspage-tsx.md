---
target: src/Web/PsiFlow.Web/src/pages/NotificationsPage.tsx
total_score: 23
p0_count: 0
p1_count: 3
---

# Critique: NotificationsPage

Design health score: 23/40.

Detector: clean for NotificationsPage.tsx and ResourcePage.tsx.

Priority issues:

1. Notification templates are modeled as generic CRUD, not operational communication workflows.
2. Test/schedule actions use fake recipients and simulated copy in a production-facing UI.
3. Retry behavior can reprocess the first log even when there is no failed log, which is unsafe and unclear.
4. Logs are rendered as unstructured prose, making failures, recipient, channel, template, and retry action hard to scan.
5. Template versioning lacks subject/body preview, channel-specific constraints, and consequence framing.

Recommended commands:

1. /impeccable harden notificacoes
2. /impeccable shape notificacoes
3. /impeccable clarify notificacoes
