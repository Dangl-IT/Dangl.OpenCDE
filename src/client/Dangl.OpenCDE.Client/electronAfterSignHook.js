const { exec } = require('child_process');
const path = require('path');

exports.default = async function (context) {

    if (process.platform !== 'win32') {
        // Signing works only on Windows
        return;
    }

    // We're calling NUKE to sign the output files before they're being packaged
    const nukeCommand = `"./build.cmd" SignExecutables -ExecutablesToSignFolder "${context.appOutDir}"`;
    const rootDirectory = path.join(__dirname, '../../../');
    const signTask = new Promise((resolve, reject) => {
        exec(nukeCommand, {
            cwd: rootDirectory
        }, (error, stdout, sdterr) => {
                console.log(stdout);
                console.log(sdterr);

                if (error) {
                    console.log(error);
                    reject(error);
                } else {
                    resolve();
                }
        });
    });

    await signTask;
}
