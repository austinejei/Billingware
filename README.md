Introduction
============

This project aims to provide a distributed, highly scalable (open source)
service that provides a developer/user intuitive tool to perform credit/debit
actions on an accounting entity. 
It includes a generic rule-parsing engine using Expression Trees. A such, you can
easily add/remove rules (and actions) to apply before an account is actually
credited/debited. Rules and actions are persisted in database.

Features
--------

1.  An REST-like HTTP API interface that performs basic banking functions such
    as credit and debit an account

2.  A read-only web user interface to view accounts, balances and transaction
    history.

3.  An SDK with which to interact with the API service (beginning with C\#)
