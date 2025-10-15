# 🔑 Getting Google Drive Credentials

## For Development

The Google Drive service account credentials are stored securely for the team.

### Option 1: Download from GitHub Release (Recommended)

1. Go to: https://github.com/zdavid01/Cinemax/releases
2. Download `cinemax-dev-credentials.zip`
3. Extract `cinemax-475115-7345803004e9.json` to this directory
4. The file is gitignored - it won't be committed

### Option 2: Get from Team Lead

Contact one of the team members to get the service account JSON:
- Bojan Velickovic
- David Zivkovic
- Dusan Trtica
- Stefan Jevtic

### Option 3: Create Your Own (Advanced)

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

