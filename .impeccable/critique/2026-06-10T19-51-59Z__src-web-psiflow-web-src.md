---
timestamp: 2026-06-10T19-51-59Z
slug: src-web-psiflow-web-src
---
# Design Health Score

| # | Heuristic | Score | Key Issue |
|---|-----------|-------|-----------|
| 1 | Visibility of System Status | 3 | Toasts and badges exist, but local fallback mode is not persistently visible after 401/405. |
| 2 | Match System / Real World | 3 | Clinical language is calm and mostly aligned, but some dev/demo labels leak into production UI. |
| 3 | User Control and Freedom | 2 | Calendar has a create-day button that does not actually open a creation flow. |
| 4 | Consistency and Standards | 3 | Strong component vocabulary, but generic ResourcePage actions create uneven workflows across domains. |
| 5 | Error Prevention | 2 | Operational actions can be fired without required context validation or confirmation. |
| 6 | Recognition Rather Than Recall | 3 | Sidebar, badges, tables and drawers help orientation; calendar adds recognition. |
| 7 | Flexibility and Efficiency | 3 | Dashboard shortcuts and monthly calendar improve efficiency, but no keyboard shortcuts or saved filters. |
| 8 | Aesthetic and Minimalist Design | 3 | Calm Clinic direction is intact; some action clusters are too dense. |
| 9 | Error Recovery | 2 | 401/405 fallback prevents breakage, but does not clearly distinguish local state from saved backend state. |
| 10 | Help and Documentation | 2 | Empty states help, but complex clinical workflows lack inline guidance. |
| **Total** | | **26/40** | **Functional but needs hardening and workflow-specific polish.** |

# Anti-Patterns Verdict

**LLM assessment**: The interface does not immediately read as AI-generated. The restrained sage palette, flat surfaces, sober copy and reusable components preserve the Calm Clinic register. The biggest AI-adjacent tell is not visual decoration, it is functional genericness: many domain flows still sit inside one ResourcePage pattern, so clinical records, sessions, notifications and online rooms can feel like the same table with different labels.

**Deterministic scan**: Detector found 1 warning in `src/Web/PsiFlow.Web/src/styles/global.css:64`, `overused-font` for Inter. This is a false positive for this project because `DESIGN.md` explicitly defines Inter as the single family and the product register favors consistency over typographic novelty.

**Visual overlays**: Browser visualization was attempted but unavailable. Playwright could not find Chrome at `C:\Users\Victor\AppData\Local\Google\Chrome\Application\chrome.exe`, so no reliable user-visible overlay was injected.

# Overall Impression

The product now feels like a credible clinical workspace rather than a scaffold. The design system is coherent and the new monthly calendar adds a real planning affordance. The largest opportunity is to replace generic action clusters with workflow-specific task surfaces, especially where clinical or operational consequence is high.

# What's Working

1. The Calm Clinic visual system is preserved. Colors, radii, type weight and flat containers stay inside the documented design rules.
2. The monthly calendar is a useful secondary object. It gives month-level orientation without stealing the Agenda page's main role.
3. The Dashboard is more actionable. Metrics and primary actions now navigate to real work areas instead of being decorative summary blocks.

# Priority Issues

## P1: Local fallback state is too quiet

**Why it matters**: A psychologist can trigger an action that appears successful locally after a 401 or 405, then later discover it was not saved server-side. For clinical operations, silent divergence is dangerous.

**Fix**: Add a persistent local-mode banner or per-row sync badge when fallback is used. Use explicit copy: `Salvo apenas neste navegador` or `Aguardando autenticação`.

**Suggested command**: `$impeccable harden`

## P1: Calendar day creation is a dead affordance

**Why it matters**: `Criar compromisso neste dia` looks like a committed action, but it does not open the appointment form with the selected date. This breaks user control and teaches distrust.

**Fix**: Pass an `onCreateForDay(date)` callback into `MonthCalendar`, open the Agenda create modal, prefill `startsAt` and `endsAt`, and focus the patient field.

**Suggested command**: `$impeccable polish`

## P2: Domain workflows are still too table-generic

**Why it matters**: Sessions, clinical records, notifications and online rooms have different cognitive tasks, but ResourcePage makes them feel mechanically identical. Users must infer which action is safe or primary.

**Fix**: Split each high-stakes domain into a task panel plus table: Sessions should show state machine actions; Records should show an editor/status rail; Notifications should show delivery health; OnlineSession should show link readiness.

**Suggested command**: `$impeccable shape`

## P2: Action clusters create decision load

**Why it matters**: Several drawers expose 5 to 8 actions with similar visual weight. More than four visible options slows clinical work and increases wrong-click risk.

**Fix**: Group actions by priority. One primary action, two secondary actions, remaining actions under `Mais ações` or contextual sections.

**Suggested command**: `$impeccable distill`

## P3: Mobile calendar density is likely tight

**Why it matters**: Seven columns with 46-64px cells can work, but on 320px screens the day cells compress to about 34px plus gaps. It may remain usable but not comfortable.

**Fix**: Add a mobile variant: weekly strip plus selected-day agenda below, or reduce gaps and min-height at `<420px` with larger tap targets preserved through padding.

**Suggested command**: `$impeccable adapt`

# Persona Red Flags

**Dr. Vitoria, busy psychologist between sessions**: The calendar helps her orient the month, but drawer action clusters force her to scan too many equal-weight buttons. The local fallback toast can disappear before she realizes an action was not saved server-side.

**Jordan, first-time clinic staff user**: The sidebar and tables are understandable, but terms like `modo local`, `MFA preparado localmente` and demo values such as `https://meet.google.com/psiflow-demo` need clearer production-safe guidance.

**Alex, power user/admin**: Dashboard shortcuts help, but there are no keyboard accelerators, saved filters, or bulk action patterns. Repeated table/drawer workflows may feel slow once the dataset grows.

# Minor Observations

- The detector's Inter warning should be ignored for this product because it is an intentional design-system decision.
- The auth page has grown into multiple flows inside one card. It is functional, but registration, recovery and MFA deserve progressive disclosure.
- `ResourcePage` has a dependency entry on `actions.length` in `useMemo` while actions are not used in table columns. This is harmless but noisy.
- Some success copy says actions were `simuladas`; production UI should use a clear local/server sync status instead.

# Questions to Consider

1. Should local fallback be a development convenience only, or a visible offline mode the product supports intentionally?
2. What is the one primary action on a session row: start, complete, record evolution, or payment?
3. Should the monthly calendar become a date picker for creating appointments, or stay purely informational?
