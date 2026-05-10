using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace GlossaryTrainer.Helpers;

public static class ProgressBarAnimation
{
    public static readonly DependencyProperty AnimatedValueProperty =
        DependencyProperty.RegisterAttached(
            "AnimatedValue",
            typeof(double),
            typeof(ProgressBarAnimation),
            new PropertyMetadata(0d, OnAnimatedValueChanged));

    public static double GetAnimatedValue(DependencyObject obj)
    {
        return (double)obj.GetValue(AnimatedValueProperty);
    }

    public static void SetAnimatedValue(DependencyObject obj, double value)
    {
        obj.SetValue(AnimatedValueProperty, value);
    }

    private static void OnAnimatedValueChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not ProgressBar progressBar)
            return;

        var animation = new DoubleAnimation
        {
            To = (double)e.NewValue,
            Duration = TimeSpan.FromMilliseconds(350),
            EasingFunction = new QuadraticEase
            {
                EasingMode = EasingMode.EaseOut
            }
        };

        progressBar.BeginAnimation(
            RangeBase.ValueProperty,
            animation,
            HandoffBehavior.SnapshotAndReplace);
    }
}