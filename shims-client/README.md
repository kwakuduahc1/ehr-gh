# SHIMS — Simple Health Information Management System

SHIMS is a **free, open-source Electronic Health Record (EHR) system** built for health facilities in low- and middle-income countries (LMICs). It is designed to run on minimum hardware, making it accessible to facilities with limited infrastructure.

Through server and database optimisations — including connection pooling, response caching, and async I/O throughout — SHIMS is capable of handling **1,000+ requests per second** on modest hardware, ensuring reliable performance in resource-constrained environments.

This repository contains the Angular client. The backend API is documented via Swagger in development builds only (`/swagger`); Swagger is disabled in production.

## Features

- **Patient Registration & Management** — Register patients, manage demographic information, search by name, hospital ID, or insurance card
- **Insurance Scheme Handling** — Associate patients with multiple insurance schemes, track coverage and expiry dates
- **Attendance Tracking** — Record and manage patient visits
- **Role-Based Access Control** — Separate access levels for Doctors, Nurses, Pharmacists, Billing, Administration, and SysAdmin
- **API Versioning** — Both versioned (`/api/v1/`) and unversioned routes supported
- **Optional Local AI for Clinical Decision Support** — Clinicians can optionally invoke AI-assisted medical decision making; fully local, no internet connection required
- **Progressive Web App (PWA)** — Installable on any device; service worker pre-caches the app shell for fast load times and resilience on slow or intermittent connections

## Progressive Web App

SHIMS ships as a PWA using the Angular service worker (`@angular/service-worker`):

- **Installable** — users can install SHIMS to their desktop or mobile home screen without an app store
- **App shell pre-cached** — all static assets (HTML, CSS, JS) are prefetched on first load; subsequent visits load instantly from cache regardless of network conditions
- **Lazy asset caching** — images and fonts are cached on first use and served from cache thereafter
- **Resilience on poor connections** — cached assets are served even when the network is slow or briefly unavailable, which is especially valuable in low-connectivity LMIC environments

> API responses are not cached by the service worker — live patient data is always fetched from the server to ensure clinical accuracy.

## AI-Assisted Medical Decision Making (Optional)

SHIMS supports optional integration with a locally-hosted AI model for clinical decision support. Key principles:

- **Clinician-initiated** — AI assistance is only invoked explicitly at the request of the treating clinician; it is never automatic
- **Fully local** — The AI model runs entirely on-premises. No patient data ever leaves the facility or touches the internet
- **Optional module** — Facilities that do not require or cannot support AI simply run SHIMS without it; all core functionality remains unaffected
- **Hardware requirement** — Running the local AI model requires capable hardware (a dedicated GPU is recommended). This is separate from the minimum hardware required for the core SHIMS stack

## Tech Stack

| Component    | Technology                        | Reason                                                                                                                                            |
| ------------ | --------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Client**   | Angular                           | Mature, component-based SPA framework with strong typing via TypeScript; well-suited for complex, data-heavy clinical UIs                         |
| **API**      | ASP.NET Core 10 (.NET 10), C#     | Best-in-class performance for a managed runtime; minimal overhead, native async support, and a mature ecosystem for building APIs                 |
| **Database** | PostgreSQL 18                     | Proven reliability, excellent performance under concurrent load, rich JSON support, and fully open source with no licensing costs                 |
| **ORM**      | Dapper                            | Lightweight micro-ORM that keeps queries close to SQL — avoids the overhead and magic of full ORMs, giving precise control over query performance |
| **Auth**     | JWT Bearer Tokens                 | Stateless authentication — no session storage required on the server, scales horizontally without shared state                                    |
| **Logging**  | Serilog (7-day rolling file logs) | Structured logging with minimal configuration; file-based logs work out of the box without requiring a logging infrastructure                     |

## Deployment

SHIMS can be deployed in two ways:

- **Docker Compose** — the recommended approach for most facilities. A `docker-compose.yml` will be provided for deployment on moderate hardware with minimal configuration.
- **Self-hosted** — individual components (client, API, database) can be deployed and managed separately to suit existing infrastructure.

For self-hosting, you will need:

- PostgreSQL 18
- A reverse proxy (e.g. Nginx, Caddy, Apache) to serve the Angular client and proxy the API

## Performance & Caching Strategy

SHIMS uses a layered caching strategy to minimise database load and maximise responsiveness, especially on constrained hardware:

| Layer                  | Mechanism                                    | Scope                                                                                        |
| ---------------------- | -------------------------------------------- | -------------------------------------------------------------------------------------------- |
| **API response cache** | ASP.NET Core response caching middleware     | Short-lived cache (e.g. 30s) on read-heavy endpoints such as patient lists                   |
| **Distributed cache**  | Valkey / Redis _(planned)_                   | Shared cache across API instances; absorbs repeated reads between response cache expirations |
| **Proxy cache**        | Nginx / Caddy / Apache cache directives      | Caches API responses at the network edge before they reach the application server            |
| **Client cache**       | Angular HTTP cache + `Cache-Control` headers | Avoids redundant API calls for stable reference data (e.g. scheme lists) within a session    |

This means a typical read request for a busy endpoint may never reach the database at all — it is served from the proxy, the distributed cache, or the application's own response cache first.

### Cache Invalidation

If data appears stale and a user needs fresh information immediately, use the appropriate steps below depending on where the cache is held:

**Browser / client cache**

- Hard reload the page: `Ctrl+Shift+R` (Windows/Linux) or `Cmd+Shift+R` (Mac)
- Or open DevTools → Network tab → check "Disable cache", then reload

**API response cache (server-side)**

- Restart the SHIMS API process — the in-memory response cache is cleared on startup
- With Docker: `docker compose restart api`

**Distributed cache (Valkey / Redis)** _(when enabled)_

- Flush all keys: `redis-cli FLUSHALL` (or `valkey-cli FLUSHALL`)
- Flush a specific database: `redis-cli FLUSHDB`
- With Docker: `docker compose exec valkey valkey-cli FLUSHALL`

**Proxy cache (Nginx / Caddy / Apache)**

- **Nginx**: delete the cache directory (configured via `proxy_cache_path`) and reload: `nginx -s reload`
- **Caddy**: Caddy does not cache by default; if using a caching plugin, run `caddy reload`
- **Apache**: clear the `mod_cache` disk storage directory and restart: `apachectl restart`

## License

MIT — see [LICENSE](../LICENSE.txt).

## AI Acknowledgement

Parts of this project were developed with the assistance of AI tools, including GitHub Copilot. All AI-generated code and content has been reviewed, tested, and validated by the project maintainers. The use of AI is intended to accelerate development, not to replace human judgement — particularly in a healthcare context where correctness and reliability are critical.

---

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 21.2.7.
