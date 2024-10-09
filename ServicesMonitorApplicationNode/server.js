const express = require("express");
const axios = require('axios');
const os = require("os");
const { exec } = require("child_process");
const app = express();
const port = 8199;
const backendServer = 'http://service2:5300/api/home';

function getIPAddress() {
    const networkInterfaces = os.networkInterfaces();
    for (const iface of Object.values(networkInterfaces)) {
        for (const addr of iface) {
            if (!addr.internal && addr.family === "IPv4") {
                return addr.address;
            }
        }
    }
    return "No IP Address found";
}

function getRunningProcesses() {
    const command = process.platform === "win32" ? "tasklist" : "ps aux";
    return execCommand(command)
}

function getAvailableDiskSpace() {
    const command = process.platform === "win32" 
        ? "wmic logicaldisk get freespace" 
        : "df -h --output=avail /";
    
    return execCommand(command);
}

function getTimeSinceLastBoot() {
    const command = process.platform === "win32" 
        ? "systeminfo | find \"System Boot Time\"" 
        : "uptime -p";

    
    return execCommand(command);
}

function execCommand(command) {
    return new Promise((resolve, reject) => {
        exec(command, (error, output) => {
            if (error) {
                return reject(error);
            }
            resolve(output);
        });
    });
}

async function fetchInfoFromService1() {
    const [ipAddress, runningProcesses, availableDiskSpace, timeSinceLastBoot] = await Promise.all([
        getIPAddress(),
        getRunningProcesses(),
        getAvailableDiskSpace(),
        getTimeSinceLastBoot(),
    ]);


    // Return all the gathered data in a structured format
    return {
        RunningProcesses: runningProcesses,
        IPAddress: ipAddress,
        AvailableDiskSpace: availableDiskSpace,
        TimeSinceLastBoot: timeSinceLastBoot,
    };
}

// API endpoint to get running processes info 
app.get("/", async (req, res) => {
    try {
        const service1Data = await fetchInfoFromService1();
        const response = await axios.get(backendServer);
        const service2Data = response.data;

        res.json({
            Service1Data: service1Data,
            Service2Data: service2Data
        });
    } catch (error) {
        res.status(500).send(`Error fetching system info: ${error}`);
    }
});

app.listen(port, () => {
    console.log(`Server running on http://localhost:${port}`);
});
