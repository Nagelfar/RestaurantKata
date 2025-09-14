# Dockerfile for Food Preparation Service

# Use Node.js official image as a base image
FROM node:20-alpine

# Set working directory
WORKDIR /app

# Copy package.json and package-lock.json (if available)
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy the application files
COPY . .

# Expose the service port
EXPOSE 3000

# Command to run the application
CMD ["npm", "start"]
