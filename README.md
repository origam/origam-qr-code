# origam-qr-code
**Origam** workflow service agent for generation QR Code.

Supported **Origam** versions: 2022.1+.

## How to use
Reference `origam-qr-code` package in your **Origam** project, add the compiled dlls to the server instance folder and adjust 
 `Origam.ServerCore.deps.json` or `Origam.Server.deps.json` depending on used Origam version.
```json
# add this to targets
"QRCoder/1.4.3": {
  "dependencies": {
    "System.Drawing.Common": "5.0.3",
    "System.Text.Encoding.CodePages": "5.0.0"
  },
  "runtime": {
    "lib/netstandard2.0/QRCoder.dll": {
     "assemblyVersion": "1.4.3.0",
     "fileVersion": "1.4.3.0"
   }
 }
}
```
```json
# add this to libraries
"QRCoder/1.4.3": {
  "type": "package",
  "serviceable": true,
  "sha512": "sha512-fWuFqjm8GTlEb2GqBl3Hi8HZZeZQwBSHxvRPtPjyNbT82H0ff0JwavKRBmMaXCno1Av6McPC8aJzri0Mj2w9Jw==",
  "path": "qrcoder/1.4.3",
  "hashPath": "qrcoder.1.4.3.nupkg.sha512"
}
```