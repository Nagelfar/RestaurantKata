import express = require("express");
import fetch from 'node-fetch';

const port = parseInt(process.env.PORT, 10) || 3000;
const myTargetConfiguration = process.env.MY_TARGET_CONFIGURATION || "http://google.com";

const app = express();

interface PersonRequestBody {}
type Persons = string[];
app.get<any, Persons, PersonRequestBody>("/persons", (req, res) => {
    res.json(["Tony","Lisa","Michael","Ginger","Food"]);
});

interface ConfigurationRequestBody {}
type Configuration = string;
app.get<any, Configuration, ConfigurationRequestBody>("/configuration", (req, res) => {
    res.send(myTargetConfiguration);
});

interface OutgoingRequestRequestBody {}
type OutgoingRequestResponse = string;
app.get<any, OutgoingRequestResponse, OutgoingRequestRequestBody>("/outgoingRequest", (req, res) => {
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