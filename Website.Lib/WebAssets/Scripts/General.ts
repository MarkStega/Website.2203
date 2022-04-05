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