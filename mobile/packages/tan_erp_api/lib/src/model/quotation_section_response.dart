//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/quotation_work_item_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'quotation_section_response.g.dart';

/// QuotationSectionResponse
///
/// Properties:
/// * [code] 
/// * [name] 
/// * [nameTh] 
/// * [nameEn] 
/// * [subtotal] 
/// * [workItems] 
@BuiltValue()
abstract class QuotationSectionResponse implements Built<QuotationSectionResponse, QuotationSectionResponseBuilder> {
  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'name')
  String? get name;

  @BuiltValueField(wireName: r'nameTh')
  String? get nameTh;

  @BuiltValueField(wireName: r'nameEn')
  String? get nameEn;

  @BuiltValueField(wireName: r'subtotal')
  double? get subtotal;

  @BuiltValueField(wireName: r'workItems')
  BuiltList<QuotationWorkItemResponse>? get workItems;

  QuotationSectionResponse._();

  factory QuotationSectionResponse([void updates(QuotationSectionResponseBuilder b)]) = _$QuotationSectionResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(QuotationSectionResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<QuotationSectionResponse> get serializer => _$QuotationSectionResponseSerializer();
}

class _$QuotationSectionResponseSerializer implements PrimitiveSerializer<QuotationSectionResponse> {
  @override
  final Iterable<Type> types = const [QuotationSectionResponse, _$QuotationSectionResponse];

  @override
  final String wireName = r'QuotationSectionResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    QuotationSectionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.name != null) {
      yield r'name';
      yield serializers.serialize(
        object.name,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.nameTh != null) {
      yield r'nameTh';
      yield serializers.serialize(
        object.nameTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.nameEn != null) {
      yield r'nameEn';
      yield serializers.serialize(
        object.nameEn,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.subtotal != null) {
      yield r'subtotal';
      yield serializers.serialize(
        object.subtotal,
        specifiedType: const FullType(double),
      );
    }
    if (object.workItems != null) {
      yield r'workItems';
      yield serializers.serialize(
        object.workItems,
        specifiedType: const FullType.nullable(BuiltList, [FullType(QuotationWorkItemResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    QuotationSectionResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required QuotationSectionResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'name':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.name = valueDes;
          break;
        case r'nameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.nameTh = valueDes;
          break;
        case r'nameEn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.nameEn = valueDes;
          break;
        case r'subtotal':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.subtotal = valueDes;
          break;
        case r'workItems':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(QuotationWorkItemResponse)]),
          ) as BuiltList<QuotationWorkItemResponse>?;
          if (valueDes == null) continue;
          result.workItems.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  QuotationSectionResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = QuotationSectionResponseBuilder();
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


