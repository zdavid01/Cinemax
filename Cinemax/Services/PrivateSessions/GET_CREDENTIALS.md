# 🔑 Getting Google Drive Credentials

## For Development

The Google Drive service account credentials are documented for team use.

### Quick Setup (Copy-Paste)

**The full JSON is available in:** `docs/GOOGLE_SERVICE_ACCOUNT.md`

1. Open the documentation:
   ```bash
   cat docs/GOOGLE_SERVICE_ACCOUNT.md
   ```

2. Copy the "Quick Setup" command from that document and run it in your terminal

3. The file will be created automatically in the correct location

**The file is gitignored** - it's safe to have locally but won't be committed.

### Alternative: Get from Team Lead

If you prefer, contact one of the team members:
- Bojan Velickovic
- David Zivkovic
- Dusan Trtica
- Stefan Jevtic

### Advanced: Create Your Own

See `PrivateSession/GOOGLE_DRIVE_SETUP.md` for instructions on creating your own service account.

---

## File Should Be Here

After obtaining the credentials, place the file here:
```
Cinemax/Services/PrivateSessions/PrivateSession/cinemax-475115-7345803004e9.json
```

The Dockerfile will automatically copy it to the container.

---

## Verification

Once the file is in place, verify it's being used:

```bash
cd Cinemax
docker-compose up -d privatesession.api
docker-compose logs privatesession.api | grep -i "google\|drive"
```

You should see the service account being loaded successfully.

