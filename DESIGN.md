---
name: PsiFlow
description: Clinical workspace for psychology — calm, precise, composed.
colors:
  primary: "oklch(0.35 0.11 140)"
  primary-hover: "oklch(0.302 0.105 140)"
  primary-soft: "oklch(0.91 0.044 140)"
  accent: "oklch(0.57 0.13 48)"
  bg: "oklch(1 0 0)"
  surface: "oklch(0.972 0.006 140)"
  surface-strong: "oklch(0.938 0.012 140)"
  sidebar: "oklch(0.185 0.034 140)"
  sidebar-soft: "oklch(0.245 0.042 140)"
  ink: "oklch(0.205 0.025 140)"
  muted: "oklch(0.435 0.018 140)"
  border: "oklch(0.885 0.012 140)"
  danger: "oklch(0.49 0.16 28)"
  danger-soft: "oklch(0.94 0.035 28)"
  warning: "oklch(0.61 0.13 75)"
  warning-soft: "oklch(0.94 0.05 75)"
  success: "oklch(0.43 0.12 145)"
  success-soft: "oklch(0.925 0.052 145)"
  info: "oklch(0.43 0.095 220)"
  info-soft: "oklch(0.93 0.035 220)"
typography:
  display:
    fontFamily: "Inter, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif"
    fontSize: "2.6rem"
    fontWeight: 800
    lineHeight: 1.04
    letterSpacing: "-0.03em"
  headline:
    fontFamily: "Inter, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif"
    fontSize: "1.55rem"
    fontWeight: 800
    lineHeight: 1.2
    letterSpacing: "-0.02em"
  title:
    fontFamily: "Inter, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif"
    fontSize: "1.25rem"
    fontWeight: 750
    lineHeight: 1.3
    letterSpacing: "normal"
  body:
    fontFamily: "Inter, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif"
    fontSize: "1rem"
    fontWeight: 500
    lineHeight: 1.5
    letterSpacing: "normal"
  label:
    fontFamily: "Inter, ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif"
    fontSize: "0.86rem"
    fontWeight: 800
    letterSpacing: "normal"
rounded:
  sm: "8px"
  md: "12px"
  lg: "16px"
  pill: "999px"
spacing:
  1: "0.25rem"
  2: "0.5rem"
  3: "0.75rem"
  4: "1rem"
  5: "1.25rem"
  6: "1.5rem"
  8: "2rem"
  10: "2.5rem"
components:
  button-primary:
    backgroundColor: "{colors.primary}"
    textColor: "{colors.bg}"
    rounded: "{rounded.sm}"
    padding: "0.7rem 1rem"
    height: "44px"
  button-primary-hover:
    backgroundColor: "{colors.primary-hover}"
  button-secondary:
    backgroundColor: "{colors.primary-soft}"
    textColor: "{colors.primary}"
    rounded: "{rounded.sm}"
    padding: "0.7rem 1rem"
    height: "44px"
  button-ghost:
    backgroundColor: "transparent"
    textColor: "{colors.primary}"
    rounded: "{rounded.sm}"
    padding: "0.7rem 1rem"
    height: "44px"
  button-danger:
    backgroundColor: "{colors.danger}"
    textColor: "{colors.bg}"
    rounded: "{rounded.sm}"
    padding: "0.7rem 1rem"
    height: "44px"
  badge:
    rounded: "{rounded.pill}"
    padding: "0.26rem 0.62rem"
    height: "28px"
  input:
    backgroundColor: "{colors.bg}"
    textColor: "{colors.ink}"
    rounded: "{rounded.sm}"
    padding: "0.74rem 0.82rem"
  card:
    backgroundColor: "{colors.bg}"
    rounded: "{rounded.md}"
    padding: "1.25rem"
  section:
    backgroundColor: "{colors.bg}"
    rounded: "{rounded.lg}"
    padding: "1.5rem"
---

# Design System: PsiFlow

## 1. Overview

**Creative North Star: "The Calm Clinic"**

The room is a consultation room, not a hospital. The interface lowers the room's temperature on entry, holds it there across the day, and hands it back to the clinician warm enough to welcome the next patient. Forest Sage is the wall. Warm Amber is the lamp. Paper White is the chart. Each does one thing; together they feel considered.

PsiFlow is a clinical workspace for psychology: agenda, sessions, records, notifications, online attendance. The system reads as one voice — composed, precise, trustworthy — without slipping into the cold hospital register or the generic SaaS register. It is human enough that a sensitive conversation can happen in the same room as the screen, clinical enough that the work is taken seriously.

This system explicitly rejects: cold hospital software (generic blue-white surfaces, dense legacy tables, sterile visual language); generic SaaS dashboard tropes (interchangeable metric cards, decorative gradients, visual noise that competes with clinical work). It is composed and human; clinical seriousness without becoming cold or bureaucratic.

**Key Characteristics:**
- One Inter family, no display/body pairing; the system reads as one voice, not a hierarchy of voices.
- Tonal layering does the depth work. Tight 8px/16px lift shadows detach modal, drawer, and toast; a restrained 12px/28px ambient shadow anchors the hero. Everything else sits flat.
- One warm note (Amber) in a room otherwise dressed in Sage. Rarity is the signature.
- Status is explicit, never decorative. State lives in badges, button text, and the row — never in a sea of icons or a legend.
- Density serves the workflow, not the screen. Prose caps at 65-75ch; tables and dense UI can run tighter.
- Every interactive control has a visible focus ring and a press feedback. Reduced motion is honored across the surface.

## 2. Colors

A restrained, tinted-neutral palette anchored on Forest Sage, with one warm accent (Warm Amber). Sage owns the room; amber earns its place; paper white carries the work. Neutrals carry 0.005-0.015 chroma toward the brand hue (140) — never toward warm by default.

### Primary
- **Forest Sage** (`oklch(0.35 0.11 140)`): the primary action color, the sidebar base, the active focus, the link. Used on the sidebar shell, primary buttons, the focus ring, and the active nav state background. Owns the room.
- **Forest Sage Hover** (`oklch(0.302 0.105 140)`): 3 lightness points and 0.005 chroma darker. Primary button hover, pressed states.
- **Forest Sage Soft** (`oklch(0.91 0.044 140)`): pale sage tint. Secondary button background, brand mark backplate, status surfaces. Quiet presence of the primary.

### Tertiary
- **Warm Amber** (`oklch(0.57 0.13 48)`): the only warm note in the system. Used for the primary focus ring, the brand mark backplate, and any single element that should be felt from across the room. Never more than one element per viewport.

### Neutral
- **Paper White** (`oklch(1 0 0)`): the body background, the card surface, the input fill. The dominant neutral.
- **Sage-tinted Paper** (`oklch(0.972 0.006 140)`): the off-white of the surface layer — timeline items, the auth gradient floor, empty states. Holds a 0.006 chroma toward the brand hue.
- **Sage-tinted Stone** (`oklch(0.938 0.012 140)`): the second neutral tier. Nested cards, the strong-state surface, the strong surface tier.
- **Deep Sage** (`oklch(0.185 0.034 140)`): the sidebar base, the modal backdrop tint, any text on a sage-tinted ground. The darkest tone in the system.
- **Sage Mist** (`oklch(0.245 0.042 140)`): the sidebar sub-tone. Sidebar helper card, brand mark backplate on the sidebar, elevated elements inside the sidebar.
- **Ink** (`oklch(0.205 0.025 140)`): primary text on light backgrounds. In the same hue family as the primary; a darker, less saturated step.
- **Muted Ink** (`oklch(0.435 0.018 140)`): secondary text, metadata, table headers, helper text. Holds 4.5:1 on paper white and on the off-white surface tier.
- **Quiet Border** (`oklch(0.885 0.012 140)`): the single border color. One hairline for cards, inputs, table dividers — no second-tier border color.

### State Colors
- **Coral** (`oklch(0.49 0.16 28)`) on **Coral Mist** (`oklch(0.94 0.035 28)`): destructive actions, delete confirmation, danger badges.
- **Amber** (`oklch(0.61 0.13 75)`) on **Amber Mist** (`oklch(0.94 0.05 75)`): warnings, attention states, draft/hold status.
- **Verdant** (`oklch(0.43 0.12 145)`) on **Verdant Mist** (`oklch(0.925 0.052 145)`): success, confirmed status, signed records.
- **Slate** (`oklch(0.43 0.095 220)`) on **Slate Mist** (`oklch(0.93 0.035 220)`): informational, neutral context, system messages.

### Named Rules
**The One Accent Rule.** Warm Amber appears on no more than one element per viewport. The active focus ring counts as one. The rarity of warm color is the system's signature.

**The Tinted Neutral Rule.** Neutrals carry 0.005-0.015 chroma toward the brand hue (140), never toward warm by default. "Warmth" lives in the amber accent and the typography, not in the body background.

**The Mist Surface Rule.** State indicators use a mist (95% lightness) background with a saturated foreground, never a saturated background. Coral Mist, Amber Mist, Verdant Mist, Slate Mist. Saturated surfaces shout; the system does not.

## 3. Typography

**Family:** Inter (with `ui-sans-serif, system-ui, -apple-system, BlinkMacSystemFont, "Segoe UI", sans-serif` fallback).

**Character:** A single sans family carries every voice. The system is not "display + body" — it is one Inter at five weights, and the room reads as one voice. There is no serif, no mono, no display face.

### Hierarchy
- **Display** (Inter 800, 2.6rem, line-height 1.04, letter-spacing -0.03em): the hero panel headline. One per page, max. `text-wrap: balance` keeps line lengths even. The 2.6rem ceiling is fixed — display sizes are not fluid.
- **Headline** (Inter 800, 1.55rem, line-height 1.2, letter-spacing -0.02em): the topbar page title. Sets the day's tone.
- **Title** (Inter 750, 1.25rem, line-height 1.3): section and modal headers, drawer headers, large card titles.
- **Body** (Inter 500, 1rem, line-height 1.5): all body, all forms, all list text. Line length capped at 65-75ch for prose; data tables and dense UI may run tighter.
- **Label** (Inter 800, 0.86rem): table headers, form labels, button text, badge text. The workhorse of the system.

### Named Rules
**The One Voice Rule.** Inter is the only family. No display faces, no mono, no secondary. The system reads as one voice, not a pairing. Add a second face only if a real reading task demands it — it does not here.

**The 800 Floor.** Labels, table headers, and form labels hold weight 800. Weight is the workhorse of hierarchy, not size alone.

**The Balance Rule.** Display and headline h-tags use `text-wrap: balance`; prose uses `text-wrap: pretty` to reduce orphans without altering line count.

## 4. Elevation

Depth comes from tonal layering. A tight lift shadow appears only on elements that need to detach from the page (modal, drawer, toast), while the hero uses a restrained ambient shadow. Cards, sections, auth, and inputs stay flat at rest. The system has three tonal tiers (paper white → sage-tinted paper → sage-tinted stone) plus two intentionally narrow shadow roles.

### Shadow Vocabulary
- **Lift Shadow** (`box-shadow: 0 8px 16px oklch(0.2 0.03 140 / 0.08)`): used on modal, detail drawer, and toast — elements that have left the page plane.
- **Ambient Hero Shadow** (`box-shadow: 0 12px 28px oklch(0.2 0.03 140 / 0.1)`): the hero panel's only lift. It gives the day's anchor presence without a wide ghost-card blur.

### Named Rules
**The Flat-By-Default Rule.** Surfaces are flat at rest. Shadow appears only as lift (modal, drawer, toast) or as a restrained ambient cue on the hero panel. Cards, sections, auth, and inputs never carry a shadow at rest.

**The Tonal Hierarchy Rule.** The three neutral tiers (paper white, sage-tinted paper, sage-tinted stone) carry depth. Do not introduce a second shadow. Do not stack shadows.

## 5. Components

### Buttons
- **Shape:** refined rectangle (8px radius). Min-height 44px. Horizontal padding 0.75rem 1.25rem. Weight 750. Text size 0.95rem. Pills are reserved for badges and profile chips.
- **Primary:** Forest Sage background, paper white text. Hover shifts to Forest Sage Hover and lifts 1px on Y over 180ms. Disabled is 55% opacity, no lift.
- **Secondary:** Forest Sage Soft background, Forest Sage text, hairline border in `oklch(0.78 0.055 140)`. Reads as a quiet primary.
- **Ghost:** transparent background, Forest Sage text. The quietest variant. Tertiary actions in dense surfaces.
- **Danger:** Coral background, paper white text. Hover shifts to a darker Coral (`oklch(0.42 0.15 28)`). Reserved for delete and irreversible actions.

### Icon Buttons
- **Shape:** full circle (50% radius). 44px square. 1px Quiet Border on paper white.
- **State:** hover shifts to the sage-tinted paper surface. Used in the topbar (notifications, mobile panel toggle) and the table action column.

### Chips (Badges)
- **Shape:** pill (999px). Min-height 28px. Horizontal padding 0.26rem 0.62rem. Weight 750. Text size 0.82rem.
- **Tones:** neutral (Sage-tinted Stone bg, Ink text), success (Verdant Mist bg, dark Verdant text), warning (Amber Mist bg, dark Amber text), danger (Coral Mist bg, dark Coral text), info (Slate Mist bg, dark Slate text). Each tone pairs a mist surface with a saturated text color — never a saturated surface.
- **State:** read-only. No hover. The badge announces, it does not invite.

### Cards / Containers
- **Section:** 16px (`--radius-lg`) radius, 1.5rem padding, paper white, 1px Quiet Border, no shadow. The page's standard container.
- **Hero panel:** 16px radius, 2rem padding, paper white, 1px Quiet Border, Ambient Hero Shadow. The single exception to the flat-by-default rule. Holds the day's anchor.
- **Care card / metric tile:** 12px (`--radius-md`) radius, 1.25rem padding, paper white, 1px Quiet Border, no shadow. The nested or compact card.
- **Empty state:** 12px radius, 2.5rem padding, sage-tinted paper background, dashed Quiet Border. Centered content with a heading and a 48ch prose line.

### Inputs / Fields
- **Style:** 1px Quiet Border, 8px (`--radius-sm`) radius, paper white. Vertical padding 0.75rem, horizontal 0.75rem. Weight 500, text size 1rem.
- **Focus:** 3px outline in `oklch(0.85 0.06 140)` (sage-tinted paper), 2px outline-offset, border shifts to Forest Sage. 180ms ease on color/border.
- **Error:** inline error chip below the field. Coral Mist background, dark Coral (`oklch(0.31 0.12 28)`) text. The chip carries the message; the field border does not turn red alone (color-blind users get the message from the text and the icon, not the border).
- **Disabled:** 55% opacity, no transition.
- **Search variant:** `input-with-icon` wraps the input with a leading lucide icon in Muted Ink. 0.75rem left padding before the icon.

### Navigation
- **Sidebar:** 280px wide, Deep Sage background, paper-white text. Brand mark (42px square, 14px radius) sits at the top.
- **Nav items:** transparent at rest, 10px radius, 0.78rem/0.9rem padding, Muted Sage icon (`oklch(0.86 0.018 140)`). Active state: paper white background, Deep Sage text, weight 700. Hover: Sage Mist background.
- **Mobile (`<920px`):** sidebar slides in from the left, behind a backdrop. Hamburger lives in the topbar.
- **Topbar:** paper white, no border at the bottom. Page title (headline weight) left, topbar actions right (icon button + profile chip).
- **Profile chip:** 44px tall, 999px radius, 1px Quiet Border, paper white. Leading avatar in 32px circle, Forest Sage Soft background, Forest Sage initials, weight 800.

### Modal / Drawer / Toast
- **Modal:** paper white, 16px radius, Lift Shadow, max-height 88vh, sticky header with 1px Quiet Border bottom. Max width 760px. Backdrop is `oklch(0.12 0.02 140 / 0.42)`. z-index 40.
- **Detail drawer:** right-anchored, 440px wide, 16px left radius, Lift Shadow, no border. Padding 1.5rem. z-index 30.
- **Toast:** bottom-right region, 360px wide, paper white, 1px border tinted by state, Lift Shadow. 12px radius. Auto-dismisses; never more than two visible at once. z-index 60.

### Form Pattern
- **Form field wrapper:** label (weight 800, 0.92rem) above the control, helper text (weight 500, Muted Ink) below. Required fields get a visible asterisk in the label color, not a tooltip.
- **Checkbox variant:** 20px box inside a 1px-bordered row, label and helper text aligned. Toggle sits right.
- **Form actions:** right-aligned at the bottom. On mobile (`<620px`), stack full-width and reverse (primary on top).

### Auth
- **Auth page:** centered 460px card on a 135° gradient from Sage-tinted Paper to Paper White. 16px radius, 1px Quiet Border, no shadow. Brand mark + h1 + helper text + form.

## 6. Do's and Don'ts

Pulled from PRODUCT.md's anti-references and the Calm Clinic philosophy.

### Do:
- **Do** keep the system in one Inter family. Add a second face only if a real reading task demands it.
- **Do** use Warm Amber on one element per viewport, never two. The accent earns its rarity.
- **Do** use tonal layering (paper white → sage-tinted paper → sage-tinted stone) for depth before reaching for shadow. Shadow is the last resort, not the first tool.
- **Do** show status in badges, button text, and the row. The interface announces; it does not require a legend.
- **Do** keep destructive actions clear and contained. A red button is fine; a row of red is not.
- **Do** give every interactive control a visible focus ring (3px sage-tinted paper outline) and a press feedback (1px Y-lift on buttons, surface tint on inputs, 180ms ease).
- **Do** honor reduced motion. Every transition respects `@media (prefers-reduced-motion: reduce)` and collapses to a crossfade or instant change.
- **Do** respect line length. Prose caps at 65-75ch; tables and dense UI may run tighter.

### Don't:
- **Don't** use a saturated state surface. Mist surfaces (95% lightness) carry the color; saturated tones carry the text. Coral Mist bg + dark Coral text. Never Coral bg + paper white text.
- **Don't** lean on a hero-metric pattern (big number / small label / supporting stats). The Dashboard already pairs the day with a patient in focus, not a metric count.
- **Don't** stack cards inside cards. Nested cards signal a missing layout decision; the workspace uses sections, two-column grids, and a single hero panel.
- **Don't** use a side-stripe border (`border-left` or `border-right` greater than 1px as a colored accent) on cards, list items, callouts, or alerts. Use full borders, background tints, leading numbers/icons, or nothing.
- **Don't** use gradient text. Decorative, never meaningful. Use a single solid color; emphasis through weight or size.
- **Don't** fall into the SaaS-cream trap. The body background is paper white with a 0.006 chroma tint toward 140, not toward warm-by-default. Warmth lives in the amber accent and the typography, not in the surface tier.
- **Don't** treat clinical data playfully. The work is sensitive. No exclamation marks, no casual interjections, no emoji, no "celebrate your progress" language.
- **Don't** add kicker eyebrows (small all-caps tracked text above every section) by reflex. The 2023-era scaffold reads as AI grammar; use it once, deliberately, if at all.
- **Don't** number sections (01 / 02 / 03) above every block. Numbers earn their place when the section IS a sequence; otherwise the scaffold reads as template reflex.
- **Don't** rely on color alone for status. The badge text, the row label, and the toast text all carry the meaning. Color is reinforcement, not the message.
- **Don't** introduce a second shadow tier. The system has one. Cards stay flat at rest; only modals, drawers, toasts, and the hero panel carry Ambient Lift.
- **Don't** invent a display face or a mono. Inter carries the room alone.
- **Don't** go off-grid with the radius scale. The scale is 8/12/16 (with 999px reserved for badges, chips, and full-circles). Other values pollute the system.
