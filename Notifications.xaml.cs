using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LAME_Hub
{
    /// <summary>
    /// Interaction logic for Notifications.xaml
    /// </summary>
    public partial class Notifications : Window
    {
        public Notifications()
        {
            InitializeComponent();
            InitialAnimations();
        }

        private void InitialAnimations()
        {
            Top = -Height;

            this.Topmost = true;

            DoubleAnimation slideDown = new DoubleAnimation
            {
                From = -Height,
                To = 100,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            DoubleAnimation Expand = new DoubleAnimation
            {
                From = Width,
                To = 400,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
                BeginTime = TimeSpan.FromSeconds(1),
            };

            Expand.Completed += reverseAnimations;

            BeginAnimation(TopProperty, slideDown);

            BeginAnimation(WidthProperty, Expand);
        }

        private void reverseAnimations(object sender, EventArgs e)
        {
            Top = Height;

            this.Topmost = true;

            DoubleAnimation slideUp = new DoubleAnimation
            {
                From = Height,
                To = -100,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut },
                BeginTime = TimeSpan.FromSeconds(5),
            };

            DoubleAnimation Compress = new DoubleAnimation
            {
                From = Width,
                To = 60,
                BeginTime = TimeSpan.FromSeconds(4),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
                Duration = TimeSpan.FromSeconds(1),
            };

            slideUp.Completed += CloseAnimations;

            BeginAnimation(TopProperty, slideUp);

            BeginAnimation(WidthProperty, Compress);

        }

        private void CloseAnimations(object sender, EventArgs e)
        {

            this.Close();
        }

    }
}
