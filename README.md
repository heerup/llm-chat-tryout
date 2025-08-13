# llm-chat-tryout
# Purpose
This is a as-simple-as-possible app for running an LLM chat system on an on-prem infrastructure for getting feedback for how usefull it is for internal use-cases.

# Features

## User landing page
Each user should be able to see a list of conversations they have started.

## Chat
Each chat session shall have an id and an Url that can be bookmarked.

## Feedback
It should be possible for the user to annotate each LLM-response with a comment as well as a up/down vote.

## Feedback readers
It shall be possible for a special group of users to read all of the feedback but not the conversations. 

## Admin logins
It shall be possible for a group of admin users to read all of the conversations as well as the feedback. 


## Statefull
All interactions must be saved so that the user can close the page at any step and no state is lost except for text entered into the chat text box.

## Queue
The system is designed to run on a single server with way to many users. Therefore it should include a queueing system that puts user-queries on a queue and tracks how big they are as well as how fast they are processed so that it can provide an estimated answer time.

# Architecture
This app should be a self-contained Aspnetcore MVC app that uses a local Ollama server to run the LLM.

It should use local files for things like chat-sessions, queue, user logins (username/passwords). Ofcourse these should be abstracted so that they can be replaced with a database later.

