# UAE Recharge App

## Overview
This repository contains the source code for the UAE Recharge mobile application. UAE Recharge is an Android application that allows users to top up the balance of their mobile phones in the UAE.

## Features
- User registration and login
- Ability to add and manage beneficiaries for easy top-up
- Top-up functionality
- Monthly limit enforcement
- Integration with API and local SQL database for data management
- Dynamic syncing of data

## User Credentials
- **User 1:**
  - Username: ahmed1
  - Phone Number: 0506901530
  - Password: password1
  - Balance: 3500 AED
  - Verified: Yes

- **User 2:**
  - Username: ahmed2
  - Phone Number: 0987654321
  - Password: password2
  - Balance: 3500 AED
  - Verified: No

## Usage
1. Clone this repository to your local machine.
2. Open the project in Visual Studio.
3. Build and run the application on an Android emulator or physical device.
4. Use the provided user credentials to log in and test the application functionality.
5.  Follow the on-screen instructions to manage beneficiaries, and perform top up beneficiaries balances.

## Data Management
- The application can work with both API and local SQL database.
- New users are created with the "IsVerified" flag set to false by default.
- User verification is performed during login.
- User balance deduction and other operations are handled securely.

## Solid Principles and Best Practices
- The application follows SOLID principles for better code organization and maintainability.
- It utilizes best practices of Android design and development.
- Logging is implemented using Serilog for efficient error tracking and debugging.

## Special List Adapter
- The application includes a special list adapter for displaying data in lists.
- This adapter optimizes list rendering and enhances user experience.
