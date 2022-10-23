using System;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;

namespace NOTE
{
    public class Animations
    {
        public void SlideOutAnimation(StackPanel element)
        {
            DoubleAnimation slideOffScreen = new DoubleAnimation()
            {
                From = 0,
                To = -SystemParameters.VirtualScreenWidth / 2,
                Duration = TimeSpan.FromSeconds(0.7)
            };
            element.RenderTransform = new TranslateTransform();
            element.RenderTransform.BeginAnimation(TranslateTransform.XProperty, slideOffScreen);
        }

        public void SlideInAnimation(StackPanel element)
        {
            DoubleAnimation slideOnScreen = new DoubleAnimation()
            {
                From = -SystemParameters.VirtualScreenWidth / 2,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.7)
            };
            element.RenderTransform = new TranslateTransform();
            element.RenderTransform.BeginAnimation(TranslateTransform.XProperty, slideOnScreen);
        }

        public void BounceDownAnimation(StackPanel element)
        {
            // set the ease function
            BounceEase bouncer = new BounceEase();
            bouncer.Bounces = 2;
            bouncer.Bounciness = 2;
            bouncer.EasingMode = EasingMode.EaseOut;

            DoubleAnimation bounceDown = new DoubleAnimation()
            {
                From = -100,
                To = 0,
                Duration = new Duration(new TimeSpan(0, 0, 2)),
                DecelerationRatio = 0.5,
                EasingFunction = bouncer
            };

            element.RenderTransform = new TranslateTransform();
            element.RenderTransform.BeginAnimation(TranslateTransform.YProperty, bounceDown);
        }

        public void BounceDownStory(StackPanel element, int beginTime)
        {
            element.Opacity = 0;

            // set the ease function
            BounceEase bouncer = new BounceEase();
            bouncer.Bounces = 2;
            bouncer.Bounciness = 2;
            bouncer.EasingMode = EasingMode.EaseOut;

            DoubleAnimation bounceDown = new DoubleAnimation()
            {
                From = -200,
                To = 0,
                Duration = TimeSpan.FromSeconds(1),
                DecelerationRatio = 0.5,
                EasingFunction = bouncer,

            };

            DoubleAnimation appear = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.1)
            };

            Storyboard storyboard = new Storyboard();

            element.RenderTransform = new TranslateTransform();

            Storyboard.SetTargetName(appear, element.Name);
            Storyboard.SetTargetProperty(appear, new PropertyPath(Control.OpacityProperty));

            Storyboard.SetTargetName(bounceDown, element.Name);
            Storyboard.SetTargetProperty(bounceDown, new PropertyPath("RenderTransform.(TranslateTransform.Y)"));

            storyboard.Children.Add(appear);
            storyboard.Children.Add(bounceDown);
            
            storyboard.BeginTime = TimeSpan.FromSeconds(beginTime);
            storyboard.Begin(element);
        }


        public void GrowInAnimation(StackPanel element)
        {
            DoubleAnimation scaleX = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new BounceEase()
            };

            DoubleAnimation scaleY = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(1),
                EasingFunction = new BounceEase()
            };

            element.RenderTransform = new ScaleTransform();
            element.RenderTransform.BeginAnimation(ScaleTransform.ScaleXProperty, scaleX);
            element.RenderTransform.BeginAnimation(ScaleTransform.ScaleYProperty, scaleY);
        }

        public void FadeInOut_Label(Label label)
        {
            DoubleAnimation fadeIn = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5)
            };

            DoubleAnimation fadeOut = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(2)
            };

            label.BeginAnimation(Label.OpacityProperty, fadeIn);
            label.BeginAnimation(Label.OpacityProperty, fadeOut);
        }

        public void FadeInOut_Image(Image image)
        {
            DoubleAnimation fadeIn = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5)
            };

            DoubleAnimation fadeOut = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(2)
            };

            image.BeginAnimation(Image.OpacityProperty, fadeIn);
            image.BeginAnimation(Image.OpacityProperty, fadeOut);
        }

        public void FadeIn_Grid(Grid grid)
        {
            DoubleAnimation fadeIn = new DoubleAnimation()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.5)
            };

            grid.BeginAnimation(Grid.OpacityProperty, fadeIn);
        }

        public void ScoresAppear(StackPanel element)
        {
            DoubleAnimation slideOnScreen = new DoubleAnimation()
            {
                From = -SystemParameters.VirtualScreenWidth / 2,
                To = 0,
                Duration = TimeSpan.FromSeconds(0)
            };
            element.RenderTransform = new TranslateTransform();
            element.RenderTransform.BeginAnimation(TranslateTransform.XProperty, slideOnScreen);
        }
    }
}
