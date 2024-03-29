## 2017.2

* Let user change the border thickness in Settings.
* Change default shortcut to CTRL/CMD + T (Q was quitting Unity on MacOS).
* QQQs can now be pinned - a pinned QQQ will jump to the top of the list.
* TODOs are not reset anymore when reimporting assets! :D
* Refresh and Process buttons shouldn't be needed anymore, but they'll be left in for the next few versions.

## 2017.1

* Minor interface adjustments
* Try keeping the list currently in memory even when reimporting assets.
  Unity will keep messing with it anyway, but in many cases it will now stay there.
* Drop support for Unity < 5.0.0.
* Rename asset.

## 2016.4

 * MacOS support!
 * Unified interface buttons to text style with border for visual clarity.
 * Added help window to Gamedev Toolbelt/CodeTODOs menu, plus links to other assets and to leave a review.
 * Refactor and partition code for clarity.

## 2016.3

 * Completely redesigned how scripts were collected/processed, as autoupdating was slowing Unity to a crawl on sizeable projects.
   Users can now process scripts and tasks on demand through buttons on the main interface (new workflow described in README).
 * Added a Welcome window to give users a few pointers on startup.

## 2016.2

 * FIX: EditorPrefs were not being initialized correctly on first start.
 * New icons and button style!
 * Buttons are now highlighted when hovered over.
 * FIX: Unity could freeze on very large projects due to auto-updating of the task list.

## 2016.1

* Initial release