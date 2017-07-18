#!/bin/bash
forever start -o /app/out.log -e /app/err.log /app/index.js
