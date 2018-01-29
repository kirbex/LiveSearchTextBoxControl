using System.Windows;
using System.Windows.Controls;

namespace LiveSearchTextBoxControl
{
    using System.Threading.Tasks;
    using System.Timers;
    using System.Windows.Input;

    public class LiveSearchTextBox : TextBox
    {
        public static readonly RoutedEvent FilterEvent = EventManager.RegisterRoutedEvent(
            nameof(Filter),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(LiveSearchTextBox));

        public static readonly RoutedEvent CancelFilteringEvent = EventManager.RegisterRoutedEvent(
            nameof(CancelFiltering),
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(LiveSearchTextBox));

        public static readonly DependencyProperty FilterTaskProperty = DependencyProperty.Register(
            nameof(FilterTask),
            typeof(Task),
            typeof(LiveSearchTextBox),
            new FrameworkPropertyMetadata(Task.CompletedTask));

        public static readonly DependencyProperty FilterCommandProperty = DependencyProperty.Register(
            nameof(FilterCommand),
            typeof(ICommand),
            typeof(LiveSearchTextBox));

        public static readonly DependencyProperty CancelFilteringCommandProperty = DependencyProperty.Register(
            nameof(CancelFilteringCommand),
            typeof(ICommand),
            typeof(LiveSearchTextBox));

        public static readonly DependencyProperty HintTextProperty = DependencyProperty.Register(
            nameof(HintText),
            typeof(string),
            typeof(LiveSearchTextBox));

        public static readonly DependencyProperty HintTemplateProperty = DependencyProperty.Register(
            nameof(HintTemplate),
            typeof(DataTemplate),
            typeof(LiveSearchTextBox),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        private Timer keyPressedDelayTimer;

        private int delayAfterPressingKeyInMilliseconds;

        static LiveSearchTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(LiveSearchTextBox),
                new FrameworkPropertyMetadata(typeof(LiveSearchTextBox)));
        }

        public LiveSearchTextBox() => InitializeTimer();

        public event RoutedEventHandler Filter
        {
            add => AddHandler(FilterEvent, value);
            remove => RemoveHandler(FilterEvent, value);
        }

        public event RoutedEventHandler CancelFiltering
        {
            add => AddHandler(CancelFilteringEvent, value);
            remove => RemoveHandler(CancelFilteringEvent, value);
        }

        public Task FilterTask
        {
            get => (Task)GetValue(FilterTaskProperty);
            set => SetValue(FilterTaskProperty, value);
        }

        public string HintText
        {
            get => (string)GetValue(HintTextProperty);
            set => SetValue(HintTextProperty, value);
        }

        public DataTemplate HintTemplate
        {
            get => (DataTemplate)GetValue(HintTemplateProperty);
            set => SetValue(HintTemplateProperty, value);
        }

        public bool WaitTillTaskIsCompleted { get; set; }

        public int DelayAfterPressingKeyInMilliseconds
        {
            get => delayAfterPressingKeyInMilliseconds;
            set
            {
                if (delayAfterPressingKeyInMilliseconds == value) return;
                delayAfterPressingKeyInMilliseconds = value;
                keyPressedDelayTimer.Interval = value;
            }
        }

        public ICommand FilterCommand
        {
            get => (ICommand)GetValue(FilterCommandProperty);
            set => SetValue(FilterCommandProperty, value);
        }

        public ICommand CancelFilteringCommand
        {
            get => (ICommand)GetValue(CancelFilteringCommandProperty);
            set => SetValue(CancelFilteringCommandProperty, value);
        }

        protected override async void OnTextChanged(TextChangedEventArgs e)
        {
            await OnTextChangedAsync().ConfigureAwait(true);
            base.OnTextChanged(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Text = string.Empty;
                RaiseFilter();
            }

            base.OnKeyDown(e);
        }

        private void InitializeTimer()
        {
            keyPressedDelayTimer = new Timer { AutoReset = false };
            keyPressedDelayTimer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            RaiseFilter();
        }

        private async Task OnTextChangedAsync()
        {
            keyPressedDelayTimer.Stop();
            RaiseCancelFilter();
            if (WaitTillTaskIsCompleted && FilterTask != null) await FilterTask.ConfigureAwait(false);
            keyPressedDelayTimer.Start();
        }

        private void RaiseFilter()
        {
            Dispatcher.Invoke(
                () =>
                    {
                        RaiseEvent(new RoutedEventArgs(FilterEvent, this));
                        FilterCommand?.Execute(null);
                    });
        }

        private void RaiseCancelFilter()
        {
            RaiseEvent(new RoutedEventArgs(CancelFilteringEvent, this));
            CancelFilteringCommand?.Execute(null);
        }
    }
}
