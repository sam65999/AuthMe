# AuthMe Enterprise Authentication System
AuthMe is a authentication software integrating with a web ui.

AuthMe is a simple, production-ready auth and licensing platform. It handles sign-in, roles (user/mod/admin), user profiles with private images, and a clean, fast UX.

Apps & Keys
- Create apps, issue/rotate/search license keys, and enforce limits by plan.
- Keep apps isolated with their own blacklists and rules.

Support
- Built-in tickets with attachments, staff replies as “AuthMe Support”, and smart email notifications.
- Reply by email is ingested back into the ticket.

Pricing & Panels
- Tiers: Beta/Free/Pro/Premium/Enterprise, enforced in the backend.
- Admin and Moderator panels with the right controls, not extra.

Security
- Strong isolation per user/app, signed URLs for private files, rate limits, input validation.

APIs & SDKs
- Clean REST APIs you can plug directly into your own app.
- Endpoints for auth/profiles, apps/keys, support, pricing, email/broadcast, analytics — all authenticated and scoped to the caller.
- Official .NET SDK (production defaults); more SDKs planned.

I built this in one month, right now its in beta so i am primarly looking for feedback, if you wanna try it out,
you can get a free account at https://authme.space

Features:
// Email-based auth with user/mod/admin roles
// Onboarding with profile management and private images
// Applications and license key management with quotas
// Per-app blacklists and rule enforcement
// Integrated support desk with brand-safe staff replies
// Transactional/broadcast emails with templates
// Tiered plans (Beta/Free/Pro/Premium/Enterprise) with enforcement
// Admin and moderator panels with scoped permissions
// Row-level isolation and signed URL storage security
// Comprehensive APIs for auth, apps/keys, support, pricing, email, analytics
