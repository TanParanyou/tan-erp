//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_input.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_item_tax_category_request.g.dart';

/// UpdateItemTaxCategoryRequest
///
/// Properties:
/// * [code] 
/// * [name] 
/// * [sortOrder] 
@BuiltValue()
abstract class UpdateItemTaxCategoryRequest implements Built<UpdateItemTaxCategoryRequest, UpdateItemTaxCategoryRequestBuilder> {
  @BuiltValueField(wireName: r'code')
  String get code;

  @BuiltValueField(wireName: r'name')
  LocalizedTextInput get name;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  UpdateItemTaxCategoryRequest._();

  factory UpdateItemTaxCategoryRequest([void updates(UpdateItemTaxCategoryRequestBuilder b)]) = _$UpdateItemTaxCategoryRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateItemTaxCategoryRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateItemTaxCategoryRequest> get serializer => _$UpdateItemTaxCategoryRequestSerializer();
}

class _$UpdateItemTaxCategoryRequestSerializer implements PrimitiveSerializer<UpdateItemTaxCategoryRequest> {
  @override
  final Iterable<Type> types = const [UpdateItemTaxCategoryRequest, _$UpdateItemTaxCategoryRequest];

  @override
  final String wireName = r'UpdateItemTaxCategoryRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateItemTaxCategoryRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'code';
    yield serializers.serialize(
      object.code,
      specifiedType: const FullType(String),
    );
    yield r'name';
    yield serializers.serialize(
      object.name,
      specifiedType: const FullType(LocalizedTextInput),
    );
    if (object.sortOrder != null) {
      yield r'sortOrder';
      yield serializers.serialize(
        object.sortOrder,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateItemTaxCategoryRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateItemTaxCategoryRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.code = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(LocalizedTextInput),
          ) as LocalizedTextInput;
          result.name.replace(valueDes);
          break;
        case r'sortOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sortOrder = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateItemTaxCategoryRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateItemTaxCategoryRequestBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


