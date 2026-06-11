---
target: src/Web/PsiFlow.Web/src/pages/LoginPage.tsx
total_score: 27
p0_count: 0
p1_count: 2
---

# Critique: LoginPage

Design health score: 27/40.

Detector: clean for LoginPage.tsx.

Priority issues:

1. MFA setup is exposed before authenticated context, which is confusing and likely to fail.
2. Registration combines psychologist and patient onboarding without enough expectation-setting, role consequences, or CRP/privacy explanation.
3. Auth errors surface raw backend messages, which may be too technical and can leak implementation detail.
4. Password requirements rely on text and data attributes; good start, but the visual state needs non-color confirmation and clear success language.
5. Recovery flow gives correct anti-enumeration copy, but lacks next-step guidance.

Recommended commands:

1. /impeccable harden auth
2. /impeccable clarify auth
3. /impeccable shape auth
