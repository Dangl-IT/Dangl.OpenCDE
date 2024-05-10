const { exec } = require("child_process");
const path = require("path");

exports.default = async function (context) {
  if (process.platform !== "win32") {
    // Signing works only on Windows
    return;
  }

  // We're calling NUKE to sign the output files before they're being packaged
  // We're hard coding another root directory here so that Nuke won't try to attempt to write to an existing build.log file
  const tempRootDirectory = path.join(__dirname, "../../nuke2");
  const nukeCommand = `"./build.cmd" SignExecutables -ExecutablesToSignFolder "${context.appOutDir}" --root "${tempRootDirectory}"`;
  const rootDirectory = path.join(__dirname, "../../../");
  const signTask = new Promise((resolve, reject) => {
    exec(
      nukeCommand,
      {
        cwd: rootDirectory,
      },
      (error, stdout, sdterr) => {
        console.log(stdout);
        console.log(sdterr);

        if (error) {
          console.log(error);
          reject(error);
        } else {
          resolve();
        }
      }
    );
  });

  await signTask;
};
