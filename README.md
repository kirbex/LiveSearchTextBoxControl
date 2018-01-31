# LiveSearchTextBoxControl
## Description
This is a text box control with live search function. It means that this control will raise events when filtering should start and when filtering should be stopped. Also, control has a watermark which can help user to understand what information should be typed.
## Differences from TextBox
### Events
 - `Filter` - raising when filtering should be start
 - `CancelFiltering` - raising when filtering should be stopped
### Commands
 - `FilterCommand` - the same as the corresponding event but for MVVM scenarious
 - `CancelFilteringCommand` - the same as the corresponding event but for MVVM scenarious
### Properties
 - `FilterTask` - when your are starting filter it is better to start it in separate thread and best to start it in task. You can bind your filter task to this property and if `WaitTillTaskIsCompleted` is set to `true` contorl will wait for completion of this task and only after that will raise new filter event.
 - `HintText` - watermark text
 - `HintTemplate` - template for watermark
 - `WaitTillTaskIsCompleted` - is set to `true` and `FilterTask` is set then control will wait for completion of the task to raise new event; if set to `false` then control will not wait completion of the `FilterTask` and will raise event again
 - `DelayAfterPressingKeyInMilliseconds` - the logic of the control is simple: when you are changing text - control waits your next action. It is good for big data when you actually don't need to filter on every user action. If you will change text after previous change before `DelayAfterPressingKeyInMilliseconds` will elapsed `Filter` event will not be raised. If you will change text after previous change after `DelayAfterPressingKeyInMillilseconds` will elapsed `Filter` event will be raised.
