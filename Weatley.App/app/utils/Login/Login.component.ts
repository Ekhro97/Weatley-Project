import { Component, Inject, OnInit } from "@angular/core";
import { RouterExtensions } from "nativescript-angular/router";
import { BarcodeScanner } from "nativescript-barcodescanner";
import { Page } from "ui/page";
import { UserService } from "~/core/Auth-services/user.service";

/* ***********************************************************
* Before you can navigate to this page from your app, you need to reference this page's module in the
* global app router module. Add the following object to the global array of routes:
* { path: "Login", loadChildren: "./Login/Login.module#LoginModule" }
* Note that this simply points the path to the page module file. If you move the page, you need to update the route too.
*************************************************************/

@Component({
	selector: "Login",
	styleUrls: ["Login.scss"],
	moduleId: module.id,
	templateUrl: "./Login.component.html"
})
export class LoginComponent implements OnInit {
	email: string;
	password: string;

	constructor(
		private page: Page,
		private routerExtensions: RouterExtensions,
		private barcodeScanner: BarcodeScanner,
		private userService: UserService
	) { }

	ngOnInit(): void {
		this.page.actionBarHidden = true;
	}

	onQrScannButtonTap(): void {
		this.barcodeScanner.scan({
			formats: "QR_CODE",
			message: "Use the volume buttons for extra light",
			showFlipCameraButton: true,
			showTorchButton: true
		}).then((result) => {
			alert({
				title: "Scan result",
				message: "Format: " + result.format + ",\nValue: " + result.text,
				okButtonText: "OK"
			});
		}, (errorMessage) => {
			console.log("No scan. " + errorMessage);
		});
	}

	navigateHome() {
		this.userService.login("ED90A54C-D224-49AA-8046-F88BA013F854");
		this.routerExtensions.navigate(["/home"], {
			transition: {
				name: "fade"
			}
		});
	}
}