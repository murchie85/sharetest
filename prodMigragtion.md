# Keyfactor Command Upgrade: 10.4.3 to 11.0  
**High-Level Summary of Required Configuration and Code Changes**

This document outlines the major areas requiring attention when upgrading from Keyfactor Command version 10.4.3 to version 11.0. The upgrade includes critical deprecations, feature replacements, and architectural changes. All items below are essential for compatibility and functionality in version 11.0.

---

## Required Configuration and Code Changes

### Migration from Classic (CMS) API
- The Classic API is fully removed in version 11.0.
- All integrations using `CMSService.asmx` or SOAP-based endpoints must be migrated to the Keyfactor REST API (`/KeyfactorAPI`).
- If immediate migration is not possible, enable `AllowDeprecatedApiCalls = true` as a temporary measure. This is not recommended for long-term use.

### Replacement of Keyfactor Java Agent
- The Java Agent is deprecated in favor of the Universal Orchestrator (UO) and Remote File Orchestrator (UFO).
- If using the Java Agent to manage JKS/PEM stores, migrate to the Remote File Orchestrator extension (available via GitHub).
- Update orchestrator registration and certificate store types accordingly.

### Script and Handler Management
- PowerShell scripts for handlers and workflows are now stored in the database, not the filesystem.
- During upgrade, scripts are auto-imported. New script registration must use the Script Management API or the user interface.
- Existing scripts should be reviewed for PowerShell 7 compatibility.

### Updated Role and Permission Model
- A new, granular permission model is introduced in version 11.0.
- Permissions previously grouped under "PKI Admin" are now divided into:
  - Certificate Authorities (Read/Modify)
  - Certificate Templates (Read/Modify)
- Review and reassign security roles for custom or restricted-access users accordingly.

### vSCEP Installation Change
- vSCEP is no longer bundled with the installer and must be deployed as a standalone application.
- If SCEP functionality is required, plan for separate server deployment and isolation.

### OAuth 2.0 / OpenID Connect (OIDC) Identity Provider Support
- OIDC support requires configuration of claims-based identity and updated configuration for admin users.
- Admin users must be defined in the new `<AdminUsers>` format.
- Scripts that interact with the Security Role APIs must be updated to use v2 endpoints.
- Note: SSH key management is currently unsupported in OIDC authentication mode.

### Certificate Store Type Updates
- The following built-in certificate store types are removed unless in use:
  - IIS
  - F5
  - AWS ACM
  - NetScaler
- Migrate to using the Universal Orchestrator and appropriate orchestrator extensions:
  - Example: IIS Orchestrator Extension, F5 Extension, etc.
- Recreate store types as necessary and install matching orchestrator extensions.

### Pre/Post Script Deprecation
- Orchestrator prescript and postscript functionality is deprecated.
- Transition automation logic to use workflow job completion handlers or event-based scripts within Keyfactor Command.

### Expiration Alert Behavior Changes
- Each expiration alert now tracks its own execution timestamp.
- Alerts should be configured to operate on independent schedules.
- Remove usage of the deprecated `LastRun` property from any custom scripts.

### Orchestrator Job and Heartbeat Configuration
- Distributed orchestrator deployments require configuration of new parameters:
  - `LockTimeout`
  - `HeartbeatInterval`
- Modify `appsettings.json` as needed for orchestrator tuning in high-availability environments.
- Ensure orchestrator agents are updated and remain compatible with version 11.0.

### Remote File Orchestrator (UFO) Considerations
- If managing file-based certificate stores (PEM, JKS, Apache, etc.), confirm usage of the Remote File Orchestrator (UFO) via Universal Orchestrator 10.4+ or 11.0.
- Discontinue use of the deprecated Java Agent and Windows Orchestrator.
- Consider transitioning to containerized UO deployments for scalable, CI/CD-integrated orchestrator management.
