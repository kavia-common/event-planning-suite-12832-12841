# Event Planner Backend (Ocean Professional)

A modern .NET 8 Web API providing:
- JWT authentication
- Event management (CRUD)
- Guest invitations and RSVP
- In-memory storage (no database required)

Docs: /docs

Quickstart
1. Run: dotnet run
2. Register: POST /api/auth/register
3. Login: POST /api/auth/login -> copy token
4. Authorize in Swagger UI: Bearer <token>
5. Create event: POST /api/events
6. Manage guests: /api/events/{eventId}/guests

Environment Variables (optional)
- JWT__Issuer
- JWT__Audience
- JWT__Key

Ocean Professional Palette
- Primary: #2563EB
- Secondary: #F59E0B
- Error: #EF4444
