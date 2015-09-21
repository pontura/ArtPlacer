 @implementation ViewController
 -(void) shareMethod: (const char *) path
 {
     NSLog(@"Sample Method Execute");
     NSString *imagePath = [NSString stringWithUTF8String:path];
  
     //        UIImage *image      = [UIImage imageNamed:imagePath];
     NSString *message   = @"Best image from my application.";
  
     NSArray *postItems  = @[message];
  
     UIActivityViewController *activityVc = [[UIActivityViewController alloc]initWithActivityItems:postItems applicationActivities:nil];
     [self presentViewController:activityVc animated:YES completion:nil];
 }
 @end
 extern "C"{
     void sampleMethod(const char * path){
         ViewController *vc = [[ViewController alloc] init];
         [vc shareMethod: path];
         [vc release];
     }
 }
