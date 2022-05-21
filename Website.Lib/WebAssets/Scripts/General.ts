import { MDCRipple } from '@material/ripple';
import { MDCDialog } from '@material/dialog';

export function setTheme(sheetName): void {
    let elem = document.getElementById("app-theme");

    if (elem) {
        elem.setAttribute("href", "_content/Website.2203/css/dioptra-" + sheetName + ".min.css");
    }
}

export function setTitle(title): void {
    document.title = title;
}

export function getClientHeight(elem): number {
    return elem.clientHeight;
}

// Would be preferable to use something like https://github.com/jimmywarting/native-file-system-adapter and enable authentication via bearer tokens.
export function downloadFile(fileUri): void {
    let a = document.createElement('a');
    a.download = "true";
    a.href = fileUri;
    a.click();
    a.remove();
}

export function instantiateErrorDialog(): void {
    new MDCRipple(document.getElementById('reload-button')!);

    var dialog = document.getElementById('reload-dialog')!;
    var container = document.getElementById('reload-container')!;
    var scrim = document.getElementById('reload-scrim')!;

    var mdcDialog = new MDCDialog(dialog);
    mdcDialog.escapeKeyAction = '';
    mdcDialog.scrimClickAction = '';

    dialog.style.display = 'flex';
    container.style.opacity = '1';
    scrim.style.opacity = '1';
}

export function scrollToTop() {
    document.body.scrollTop = 0; // For Safari
    document.documentElement.scrollTop = 0; // For Chrome, Firefox, IE and Opera
}