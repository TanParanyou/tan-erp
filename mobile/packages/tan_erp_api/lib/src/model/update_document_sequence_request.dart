//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_document_sequence_request.g.dart';

/// UpdateDocumentSequenceRequest
///
/// Properties:
/// * [prefix] 
/// * [formatPattern] 
/// * [resetPeriod] 
/// * [padding] 
/// * [isBranchSpecific] 
@BuiltValue()
abstract class UpdateDocumentSequenceRequest implements Built<UpdateDocumentSequenceRequest, UpdateDocumentSequenceRequestBuilder> {
  @BuiltValueField(wireName: r'prefix')
  String? get prefix;

  @BuiltValueField(wireName: r'formatPattern')
  String get formatPattern;

  @BuiltValueField(wireName: r'resetPeriod')
  String get resetPeriod;

  @BuiltValueField(wireName: r'padding')
  int? get padding;

  @BuiltValueField(wireName: r'isBranchSpecific')
  bool? get isBranchSpecific;

  UpdateDocumentSequenceRequest._();

  factory UpdateDocumentSequenceRequest([void updates(UpdateDocumentSequenceRequestBuilder b)]) = _$UpdateDocumentSequenceRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateDocumentSequenceRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateDocumentSequenceRequest> get serializer => _$UpdateDocumentSequenceRequestSerializer();
}

class _$UpdateDocumentSequenceRequestSerializer implements PrimitiveSerializer<UpdateDocumentSequenceRequest> {
  @override
  final Iterable<Type> types = const [UpdateDocumentSequenceRequest, _$UpdateDocumentSequenceRequest];

  @override
  final String wireName = r'UpdateDocumentSequenceRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateDocumentSequenceRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.prefix != null) {
      yield r'prefix';
      yield serializers.serialize(
        object.prefix,
        specifiedType: const FullType.nullable(String),
      );
    }
    yield r'formatPattern';
    yield serializers.serialize(
      object.formatPattern,
      specifiedType: const FullType(String),
    );
    yield r'resetPeriod';
    yield serializers.serialize(
      object.resetPeriod,
      specifiedType: const FullType(String),
    );
    if (object.padding != null) {
      yield r'padding';
      yield serializers.serialize(
        object.padding,
        specifiedType: const FullType(int),
      );
    }
    if (object.isBranchSpecific != null) {
      yield r'isBranchSpecific';
      yield serializers.serialize(
        object.isBranchSpecific,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateDocumentSequenceRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateDocumentSequenceRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'prefix':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.prefix = valueDes;
          break;
        case r'formatPattern':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.formatPattern = valueDes;
          break;
        case r'resetPeriod':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.resetPeriod = valueDes;
          break;
        case r'padding':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.padding = valueDes;
          break;
        case r'isBranchSpecific':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isBranchSpecific = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateDocumentSequenceRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateDocumentSequenceRequestBuilder();
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


