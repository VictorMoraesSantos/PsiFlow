# Clinical Records Encryption

For the MVP, clinical content is encrypted client-side before it reaches ClinicalRecords.

The backend persists only the encryption envelope per clinical field:

- `ciphertext`
- `nonce`
- `tag`

The backend must not log or emit clinical plaintext. Key management and rotation stay outside the ClinicalRecords service boundary for this MVP and must be handled by the client/key-management layer before production rollout.
