import express from "express";
import fetch from 'node-fetch';

const port = parseInt(process.env.PORT, 10) || 3000;
const myTargetConfiguration = process.env.MY_TARGET_CONFIGURATION || "http://google.com";

const app = express();

app.get("/persons", (req, res, next) => {
    res.json(["Tony","Lisa","Michael","Ginger","Food"]);
});

app.get("/configuration", (req, res, next) => {
    res.send(myTargetConfiguration);
});

app.get("/outgoingRequest", (req, res, next) => {
    fetch(myTargetConfiguration)
        .then(res => res.text()) 
        .then(text => {
            res.send(text);
        })
        .catch(err => {
            console.log(err);
        });    
});

app.listen(port, () => {
    console.log(`Server running on port ${port}`);
});