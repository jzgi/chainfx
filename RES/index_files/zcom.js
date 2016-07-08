(function(){var ZCOM=function(domain){this.domain=domain||'.zurb.com';this.leadId=findCookie('intercom-id');this.email=findCookie('zurb-intercom-email');var queue=findCookie('zcom-queue');if(queue){this.queue=JSON.parse(queue);}else{this.queue=[];}
this.bindEvents(document);this.runQueue();};ZCOM.prototype.bindEvents=function(rootNode){var eventElements=rootNode.querySelectorAll('[data-zcom-event]');var self=this;for(var i=0;i<eventElements.length;i++){if(eventElements[i].getAttribute('data-has-zcom-event-handler')===null){eventElements[i].addEventListener('click',function(){self.createEvent(this.dataset.zcomEvent);});eventElements[i].setAttribute('data-has-zcom-event-handler',1)}};var attributeElements=rootNode.querySelectorAll('[data-zcom-attribute]');for(var i=0;i<attributeElements.length;i++){if(attributeElements[i].getAttribute('data-has-zcom-attribute-handler')===null){attributeElements[i].addEventListener('click',function(){self.createAttribute({name:this.dataset.zcomAttribute});});attributeElements[i].setAttribute('data-has-zcom-attribute-handler',1)}};var emailElements=rootNode.querySelectorAll('input[type="email"]');var self=this;for(var i=0;i<emailElements.length;i++){var form=emailElements[i].form;var elem=emailElements[i];if(form.getAttribute('data-has-zcom-email-handler')===null){form.addEventListener('submit',function(){var email=elem.value;if(!self.hasEmail()&&validateEmail(email)){self.setEmail(email);}});form.setAttribute('data-has-zcom-email-handler',1);}}};ZCOM.prototype.createEvent=function(eventName,data){data=data||{}
data.event_name=eventName;data.created_at=parseInt(Date.now()/ 1000);
this.queueUpdate(data);};ZCOM.prototype.createAttribute=function(data){this.queueUpdate({custom_attributes:data})};ZCOM.prototype.queueUpdate=function(data){this.queue.push(data)
this.saveQueue();};ZCOM.prototype.saveQueue=function(){setCookie('zcom-queue',JSON.stringify(this.queue),720,this.domain)};ZCOM.prototype.runQueue=function(data){if(this.queue.length){var self=this;this._sendUpdate(this.queue[0],function(){self.queue.shift();self.saveQueue();self.runQueue();})}else{window.setTimeout(this.runQueue.bind(this),1000);}};ZCOM.prototype._sendUpdate=function(data,cb){var xhttp=new XMLHttpRequest();var data=this.addUserInfo(data);xhttp.open('POST','https://intercom.zurb.com/updates',true);xhttp.setRequestHeader('Content-type','application/json');xhttp.onreadystatechange=function(){if(xhttp.readyState==4&&xhttp.status==200){if(cb){cb(JSON.parse(xhttp.response));}}}
xhttp.send(JSON.stringify(data));};ZCOM.prototype.setEmail=function(email){if(this.email!==email){this.email=email;this.queueUpdate({});}
setCookie('zurb-intercom-email',this.email,730,this.domain);};ZCOM.prototype.hasEmail=function(){return!!this.email;};ZCOM.prototype.addUserInfo=function(data){var results={};for(var key in data){if(data.hasOwnProperty(key)){results[key]=data[key];}}
if(this.hasEmail()){results.email=this.email;}
results.user_id=findCookie('intercom-id');return results;};function decodeCookie(){var cookieParts=document.cookie.split(';'),cookies={};for(var i=0;i<cookieParts.length;i++){var name_value=cookieParts[i],equals_pos=name_value.indexOf('='),name=unescape(name_value.slice(0,equals_pos)).trim(),value=unescape(name_value.slice(equals_pos+ 1));cookies[':'+ name]=value;}
return cookies;};function findCookie(searchWord){var cookies=decodeCookie();for(name in cookies){var value=cookies[name];if(name.indexOf(':'+ searchWord)==0){return value;}}};function setCookie(cname,cvalue,exdays,domain){var d=new Date();d.setTime(d.getTime()+(exdays*24*60*60*1000));var expires='expires='+ d.toUTCString();document.cookie=cname+'='+ cvalue+'; '+ expires+'; domain='+ domain+'; path=/';};function validateEmail(email){var re=/^(([^<>()[\]\.,;:\s@\"]+(\.[^<>()[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;return re.test(email);};function bindReady(handler){var called=false
function ready(){if(called)return
called=true
handler()}
if(document.addEventListener){document.addEventListener("DOMContentLoaded",ready,false)}else if(document.attachEvent){try{var isFrame=window.frameElement!=null}catch(e){}
if(document.documentElement.doScroll&&!isFrame){function tryScroll(){if(called)return
try{document.documentElement.doScroll("left")
ready()}catch(e){setTimeout(tryScroll,10)}}
tryScroll()}
document.attachEvent("onreadystatechange",function(){if(document.readyState==="complete"){ready()}})}
if(window.addEventListener)
window.addEventListener('load',ready,false)
else if(window.attachEvent)
window.attachEvent('onload',ready)
else{var fn=window.onload
window.onload=function(){fn&&fn()
ready()}}}
function fireZcomAvailableEvent(){var event;if(document.createEvent){event=document.createEvent("HTMLEvents");event.initEvent("zcomavailable",true,true);}else{event=document.createEventObject();event.eventType="zcomavailable";}
event.eventName="zcomavailable";if(document.createEvent){window.dispatchEvent(event);}else{window.fireEvent("on"+ event.eventType,event);}}
bindReady(function(){window.ZCOM=new ZCOM();fireZcomAvailableEvent();});}());