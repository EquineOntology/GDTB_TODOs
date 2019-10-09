Hello! Please note that this package is no longer in active development.

# TODOs

## WHAT IS TODOS?

TODOs is a small extension for Unity which collects any task you in your code and
displays them in Unity, to help you keep track of what you still need to do, where,
and the priority each task.

Every programmer is familiar with shortcuts, hacks, "throw NotImplementedException"
and so on. With TODOs, you can simply add a small code to a comment in which you
describe the task, and get back to it when it's time without fear of forgetting what
you were doing, or of leaving the hack untouched altogether!

## HOW DOES IT WORK?

The first time you start TODOs, a lish should be populated automatically for you.
It might be empty unless you start your comments with "QQQ" already, no worries if
it is!

As for formatting the comments themselves as tasks, you can simply write something like
"//QQQ This gameobject should be grabbed by the code instead of dragged and dropped"
Note that QQQ (the "token") can be changed in the Preferences window.

Additionally, you can also exclude files or entire folders from the task list! You can
find more information in the exclude.txt file.

If for some reason TODOs doesn't populate the list on its own, you can follow this process:

1) The first time you start TODOs, click on "Process scripts" (or the button with
four circular arrows), which will create a list of all scripts in the project.
2) After that's done, click on "Refresh" (the button with a single circular arrow),
which will look through these scripts for tasks.
3)After that first time, you will only need to click "Refresh tasks" when you want to
update the list of task. If there are issues, the first thing you should do is to
process scripts again.