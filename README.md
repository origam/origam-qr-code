# origam-qr-code
**Origam** workflow service agent for generation QR Code.

Supported **Origam** versions: 2025.6+.

## How to use
Reference `origam-qr-code` package in your **Origam** project, add the compiled dlls to the server instance folder and adjust `Origam.Server.deps.json`.
```json
# add this to targets
  "Origam.QRCode/1.0.0": {
	"dependencies": {
	  "Origam.Service.Core": "1.1.0",
	  "QRCoder": "1.4.3",
	  "SixLabors.ImageSharp": "3.1.12"
	},
	"runtime": {
	  "Origam.QRCode.dll": {}
	}
  },
  "QRCoder/1.4.3": {
	"runtime": {
	  "lib/net6.0/QRCoder.dll": {
		"assemblyVersion": "1.4.3.0",
		"fileVersion": "1.4.3.0"
	  }
	}
  }
```
```json
# add this to libraries
"Origam.QRCode/1.0.0": {
  "type": "project",
  "serviceable": false,
  "sha512": ""
},
"QRCoder/1.4.3": {
  "type": "package",
  "serviceable": true,
  "sha512": "sha512-fWuFqjm8GTlEb2GqBl3Hi8HZZeZQwBSHxvRPtPjyNbT82H0ff0JwavKRBmMaXCno1Av6McPC8aJzri0Mj2w9Jw==",
  "path": "qrcoder/1.4.3",
  "hashPath": "qrcoder.1.4.3.nupkg.sha512"
}
```