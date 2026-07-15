using Microsoft.AspNetCore.Razor.TagHelpers;

namespace thecodemechanic.Pages.Components;

[HtmlTargetElement("drip-sidebar")]
public class DripSidebarTagHelper : TagHelper
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        output.Attributes.SetAttribute("class", "relative z-50");
        output.Attributes.SetAttribute("x-data", ""); // ← this is the key
// or even better for clarity:
// output.Attributes.SetAttribute("x-data", "{}");
        output.TagMode = TagMode.StartTagAndEndTag;

        output.Content.SetHtmlContent("""
                                      <template x-teleport="body">
                                             <div x-show="$store?.content?.modalOpen" @keydown.window.escape="$store.content.cancel()"
                                                  class="relative z-[99]">

                                                 <!-- Backdrop -->
                                                 <div x-show="$store?.content?.modalOpen" x-transition.opacity.duration.300ms
                                                      x-on:click="$store.content.cancel()" class="fixed inset-0 bg-black/60">
                                                 </div>

                                                 <!-- Slide-over -->
                                                 <div class="overflow-hidden fixed inset-0">
                                                     <div class="overflow-hidden absolute inset-0">
                                                         <div class="flex fixed inset-y-0 right-0 pt-11 pl-10 max-w-full">
                                                             <div x-show="$store?.content?.modalOpen"
                                                                  x-transition:enter="transform transition ease-in-out duration-500 sm:duration-700"
                                                                  x-transition:enter-start="translate-x-full" x-transition:enter-end="translate-x-0"
                                                                  x-transition:leave="transform transition ease-in-out duration-500 sm:duration-700"
                                                                  x-transition:leave-start="translate-x-0" x-transition:leave-end="translate-x-full"
                                                                  class="w-screen max-w-md">

                                                                 <div class="flex h-full flex-col overflow-y-auto border-l shadow-2xl"
                                                                      style="background-color: #111827; border-color: rgba(226, 232, 240, 0.1);">

                                                                     <!-- Header -->
                                                                     <div class="px-5 pt-5">
                                                                         <div class="flex items-start justify-between pb-2">
                                                                             <div>
                                                                                 <h2 class="text-base font-semibold" style="color: #e2e8f0;">                                                
                                                                                     <span x-text="
                                                                                     $store.content.editingType === 'images' ? 'Edit Image (Tag helper ed.)' :
                                                                                     $store.content.editingType === 'videos' ? 'Edit Video (Tag helper ed.)' :
                                                                                     'Edit Content (Tag helper ed.)'
                                                                                 "></span>
                                                                                 </h2>
                                                                                 <p class="font-mono text-xs" style="color: #64748b;"
                                                                                    x-text="$store.content.editingKey"></p>
                                                                             </div>
                                                                             <button x-on:click="$store.content.cancel()"
                                                                                     class="flex items-center gap-1 rounded-md border px-3 py-1.5 text-xs font-medium transition-colors"
                                                                                     style="border-color: rgba(226, 232, 240, 0.2); color: #94a3b8;">
                                                                                 Close
                                                                             </button>
                                                                         </div>
                                                                     </div>

                                                                     <!-- Body -->
                                                                     <div class="flex-1 space-y-5 px-5 pt-4 pb-6">


                                                                         <!-- Editor input (Markdown for content, URL for images/videos) -->
                                                                         <div>
                                                                             <label class="mb-1 block text-sm font-medium" style="color: #94a3b8;"
                                                                                    x-text="
                                                                                 $store.content.editingType === 'images' ? 'Image URL' :
                                                                                 $store.content.editingType === 'videos' ? 'Video URL' :
                                                                                 'Markdown'
                                                                             ">
                                                                             </label>

                                                                             <template x-if="$store.content.editingType === 'content'">
                                                                                         <textarea x-model="$store.content.draft" rows="10"
                                                                                                   class="w-full rounded-md border p-3 font-mono text-sm focus:outline-none"
                                                                                                   style="background-color: #0a0a0a; border-color: rgba(226, 232, 240, 0.15); color: #e2e8f0;"
                                                                                                   placeholder="Enter markdown..."></textarea>
                                                                             </template>

                                                                             <template x-if="$store.content.editingType !== 'content'">
                                                                                 <textarea x-model="$store.content.draft" rows="3"
                                                                                           class="w-full rounded-md border p-3 font-mono text-sm focus:outline-none"
                                                                                           style="background-color: #0a0a0a; border-color: rgba(226, 232, 240, 0.15); color: #e2e8f0;"
                                                                                           :placeholder="$store.content.editingType === 'images' ? 'https://example.com/image.jpg' : 'https://example.com/video.mp4'"></textarea>
                                                                             </template>

                                                                             <!-- File upload (only for images & videos) -->
                                                                             <template
                                                                                     x-if="$store.content.editingType === 'images' || $store.content.editingType === 'videos'">
                                                                                 <div class="mt-3">
                                                                                     <label class="mb-1 block text-sm font-medium" style="color: #94a3b8;">
                                                                                         Or upload a file
                                                                                     </label>
                                                                                     <input type="file"
                                                                                            accept="image/*,video/*"
                                                                                            class="block w-full text-sm"
                                                                                            style="color: #94a3b8;"
                                                                                            @change="
                                                        const file = $event.target.files[0];
                                                        if (file) $store.content.uploadFile(file);
                                                        $event.target.value = '';   // allow re-selecting same file
                                                    ">
                                                                                 </div>
                                                                             </template>
                                                                         </div>

                                                                         <!-- Preview -->
                                                                         <div>
                                                                             <label class="mb-1 block text-sm font-medium"
                                                                                    style="color: #94a3b8;">Preview</label>

                                                                             <!-- Preview section – show live image/video when not content -->
                                                                             <template x-if="$store.content.editingType === 'content'">
                                                                                 <div class="prose prose-sm max-w-none min-h-[120px] rounded-md border p-4"
                                                                                      style="background-color: #0a0a0a; border-color: rgba(226, 232, 240, 0.15); color: #e2e8f0;"
                                                                                      x-html="$store.content.renderMarkdown($store.content.draft)">
                                                                                 </div>
                                                                             </template>

                                                                             <template x-if="$store.content.editingType === 'images'">
                                                                                 <img :src="$store.content.draft" class="max-w-full rounded" alt="preview">
                                                                             </template>

                                                                             <template x-if="$store.content.editingType === 'videos'">
                                                                                 <video :src="$store.content.draft" controls
                                                                                        class="max-w-full rounded"></video>
                                                                             </template>

                                                                         </div>

                                                                     </div>

                                                                     <!-- Footer -->
                                                                     <div class="flex justify-end gap-3 border-t px-5 py-4"
                                                                          style="border-color: rgba(226, 232, 240, 0.1);">
                                                                         <button x-on:click="$store.content.cancel()"
                                                                                 class="rounded-md border px-4 py-2 text-sm transition-colors"
                                                                                 style="border-color: rgba(226, 232, 240, 0.2); color: #94a3b8;">
                                                                             Cancel
                                                                         </button>
                                                                         <button x-on:click="$store.content.save()"
                                                                                 class="rounded-md px-4 py-2 text-sm font-medium transition-colors"
                                                                                 style="background-color: #11aaff; color: white;">
                                                                             Save Changes
                                                                         </button>
                                                                     </div>

                                                                 </div>
                                                             </div>
                                                         </div>
                                                     </div>
                                                 </div>
                                             </div>
                                         </template>
                                      """);
    }
}